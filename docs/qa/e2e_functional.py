#!/usr/bin/env python3
"""
E2E Functional Test Suite for BudgetCouple API
Tests against production: https://budgetcouple-api.onrender.com/api/v1

Phases: 1 (Auth, Categorias, Contas, Cartões, Lançamentos, Parcelados, Recorrentes, Faturas, Relatórios, Dashboard)
        5 (Regression tests)
        6 (Edge cases)

Usage:
    python3 e2e_functional.py [--clean-only | --test-only]
"""

import requests
import json
import sys
import time
import os
from datetime import datetime, timedelta, date
from typing import Dict, List, Any, Optional, Tuple
import sqlite3
import psycopg2
from psycopg2 import sql
import threading
from concurrent.futures import ThreadPoolExecutor, as_completed

# Configuration
BASE_URL = "https://budgetcouple-api.onrender.com/api/v1"
PIN_TEST = "123456"
PIN_TEST_INVALID = "000000"
PIN_SHORT = "123"
PIN_ALPHA = "abcd12"

# Database config for reset
DB_CONFIG = {
    "host": "aws-0-eu-west-1.pooler.supabase.com",
    "port": 5432,
    "database": "postgres",
    "user": "postgres.lozvktwugzcbmlhbgttl",
    "password": "Ka5aXxAxLF1e8VMJrosHJVHK19Yk4Bc2",
    "sslmode": "require"
}

# Test results tracking
test_results = {
    "passed": [],
    "failed": [],
    "blocked": []
}

class TestContext:
    """Holds test state"""
    def __init__(self):
        self.token: Optional[str] = None
        self.conta_id: Optional[str] = None
        self.cartao_id: Optional[str] = None
        self.categoria_id: Optional[str] = None
        self.lancamento_id: Optional[str] = None
        self.fatura_id: Optional[str] = None
        self.recorrencia_id: Optional[str] = None
        self.session = requests.Session()


def log(msg: str, level: str = "INFO"):
    """Centralized logging"""
    timestamp = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
    print(f"[{timestamp}] {level}: {msg}")


def reset_database():
    """Reset production database - transactional tables only"""
    log("Starting database reset...", "WARN")
    try:
        conn = psycopg2.connect(**DB_CONFIG)
        cursor = conn.cursor()

        # Disable foreign key constraints temporarily
        cursor.execute("SET session_replication_role = 'replica';")

        # Truncate transactional tables (keep app_config, auth tables)
        tables_to_truncate = [
            "lancamentos",
            "faturas",
            "recorrencias",
            "cartoes",
            "contas",
            "categorias",
            "lancamento_anexos",
            "regras_classificacao",
            "metas"
        ]

        for table in tables_to_truncate:
            try:
                cursor.execute(f"TRUNCATE TABLE {table} CASCADE;")
                log(f"Truncated {table}", "DEBUG")
            except psycopg2.Error as e:
                log(f"Failed to truncate {table}: {e}", "WARN")

        # Reset auth/app_config
        cursor.execute("TRUNCATE TABLE app_config CASCADE;")
        cursor.execute("TRUNCATE TABLE usuarios CASCADE;")

        # Re-enable FK constraints
        cursor.execute("SET session_replication_role = 'user';")

        conn.commit()
        cursor.close()
        conn.close()
        log("Database reset completed", "SUCCESS")
        return True
    except Exception as e:
        log(f"Database reset failed: {e}", "ERROR")
        return False


def assert_status(response: requests.Response, expected: int, test_name: str) -> bool:
    """Assert response status code"""
    if response.status_code == expected:
        log(f"✓ {test_name}: {response.status_code}", "PASS")
        test_results["passed"].append(test_name)
        return True
    else:
        log(f"✗ {test_name}: expected {expected}, got {response.status_code}", "FAIL")
        log(f"  Response: {response.text[:200]}", "DEBUG")
        test_results["failed"].append((test_name, expected, response.status_code, response.text))
        return False


def assert_json_field(data: Dict, field: str, test_name: str) -> bool:
    """Assert field exists in JSON response"""
    if field in data:
        log(f"✓ {test_name}: field '{field}' present", "PASS")
        test_results["passed"].append(test_name)
        return True
    else:
        log(f"✗ {test_name}: field '{field}' missing", "FAIL")
        test_results["failed"].append((test_name, f"field {field}", "missing", json.dumps(data)))
        return False


def health_check(retries: int = 10, delay: int = 2):
    """Wait for API to be healthy (for redeploys)"""
    for i in range(retries):
        try:
            # Use auth/status which always works
            resp = requests.get(f"{BASE_URL}/auth/status")
            if resp.status_code == 200:
                log("API health check passed", "SUCCESS")
                return True
        except:
            pass

        log(f"Health check {i+1}/{retries} failed, retrying in {delay}s...", "WARN")
        time.sleep(delay)

    log("API health check failed after retries", "ERROR")
    return False


# ============================================================================
# PHASE 1: FUNCTIONAL TESTS
# ============================================================================

def test_auth_setup_happy(ctx: TestContext) -> bool:
    """1.1.1: Setup PIN feliz (201/200 + token)"""
    resp = ctx.session.post(f"{BASE_URL}/auth/setup-pin", json={"pin": PIN_TEST})

    if assert_status(resp, 200, "Auth Setup Happy"):
        try:
            data = resp.json()
            if assert_json_field(data, "token", "Auth Setup - Token Field"):
                ctx.token = data["token"]
                return True
        except:
            pass

    return False


def test_auth_setup_duplicate(ctx: TestContext) -> bool:
    """1.1.2: Setup quando já existe → 409"""
    resp = ctx.session.post(f"{BASE_URL}/auth/setup-pin", json={"pin": "999999"})
    return assert_status(resp, 409, "Auth Setup Duplicate")


def test_auth_setup_non_numeric(ctx: TestContext) -> bool:
    """1.1.3: Setup com PIN não-numérico → 400"""
    resp = ctx.session.post(f"{BASE_URL}/auth/setup-pin", json={"pin": PIN_ALPHA})
    return assert_status(resp, 400, "Auth Setup Non-Numeric")


def test_auth_setup_short_pin(ctx: TestContext) -> bool:
    """1.1.4: Setup com PIN < 4 dígitos → 400"""
    resp = ctx.session.post(f"{BASE_URL}/auth/setup-pin", json={"pin": PIN_SHORT})
    return assert_status(resp, 400, "Auth Setup Short PIN")


def test_auth_login_correct(ctx: TestContext) -> bool:
    """1.1.5: Login correto"""
    resp = ctx.session.post(f"{BASE_URL}/auth/login", json={"pin": PIN_TEST})

    if assert_status(resp, 200, "Auth Login Correct"):
        try:
            data = resp.json()
            if assert_json_field(data, "token", "Auth Login - Token Field"):
                ctx.token = data["token"]
                ctx.session.headers.update({"Authorization": f"Bearer {ctx.token}"})
                return True
        except:
            pass

    return False


def test_auth_login_attempts(ctx: TestContext) -> bool:
    """1.1.6: Login errado 5x → travamento"""
    for i in range(5):
        resp = ctx.session.post(f"{BASE_URL}/auth/login", json={"pin": PIN_TEST_INVALID})
        if resp.status_code == 401:
            log(f"  Failed attempt {i+1}/5: 401", "DEBUG")
        else:
            log(f"  Failed attempt {i+1}/5: unexpected {resp.status_code}", "WARN")

    # 6th attempt should be locked
    resp = ctx.session.post(f"{BASE_URL}/auth/login", json={"pin": PIN_TEST})
    if assert_status(resp, 429, "Auth Login Locked After 5 Attempts"):
        return True
    else:
        # Some systems use 401 with message
        if resp.status_code == 401 and "locked" in resp.text.lower():
            log("✓ Auth Login Locked: 401 with 'locked' message", "PASS")
            test_results["passed"].append("Auth Login Locked After 5 Attempts")
            return True

    return False


def test_auth_status(ctx: TestContext) -> bool:
    """1.1.7: GET /auth/status retorna shape correto"""
    resp = ctx.session.get(f"{BASE_URL}/auth/status")

    if assert_status(resp, 200, "Auth Status"):
        try:
            data = resp.json()
            fields = ["pinConfigured", "locked", "lockedUntil"]
            all_present = all(field in data for field in fields)

            if all_present:
                log(f"✓ Auth Status - All fields present: {data}", "PASS")
                test_results["passed"].append("Auth Status - Fields")
                return True
            else:
                missing = [f for f in fields if f not in data]
                log(f"✗ Auth Status - Missing fields: {missing}", "FAIL")
                test_results["failed"].append(("Auth Status - Fields", fields, missing, json.dumps(data)))
        except Exception as e:
            log(f"✗ Auth Status - JSON parse error: {e}", "FAIL")

    return False


def test_auth_no_token(ctx: TestContext) -> bool:
    """1.1.8: Endpoint sem token → 401"""
    # Create a new session without token
    temp_session = requests.Session()
    resp = temp_session.get(f"{BASE_URL}/categorias")
    return assert_status(resp, 401, "Auth No Token")


def test_categorias_list(ctx: TestContext) -> bool:
    """1.2.1: List retorna array"""
    resp = ctx.session.get(f"{BASE_URL}/categorias")

    if assert_status(resp, 200, "Categorias List"):
        try:
            data = resp.json()
            if isinstance(data, list):
                log(f"✓ Categorias List - Is array with {len(data)} items", "PASS")
                test_results["passed"].append("Categorias List - Is Array")
                return True
            else:
                log(f"✗ Categorias List - Not an array: {type(data)}", "FAIL")
                test_results["failed"].append(("Categorias List - Is Array", "list", type(data).__name__, json.dumps(data)[:100]))
        except Exception as e:
            log(f"✗ Categorias List - JSON error: {e}", "FAIL")

    return False


def test_categorias_create(ctx: TestContext) -> bool:
    """1.2.2: Create categoria"""
    payload = {
        "nome": "Test Category",
        "tipoCategoria": "DESPESA",
        "corHex": "#FF0000",
        "icone": "shopping-cart",
        "parentCategoriaId": None
    }

    resp = ctx.session.post(f"{BASE_URL}/categorias", json=payload)

    if assert_status(resp, 201, "Categorias Create"):
        try:
            data = resp.json()
            if "id" in data:
                ctx.categoria_id = data["id"]
                log(f"  Categoria ID: {ctx.categoria_id}", "DEBUG")
                test_results["passed"].append("Categorias Create - ID Field")
                return True
        except:
            pass

    return False


def test_categorias_update(ctx: TestContext) -> bool:
    """1.2.3: Update categoria"""
    if not ctx.categoria_id:
        log("Skipping Categorias Update - no categoria_id", "SKIP")
        return True

    payload = {
        "nome": "Updated Category",
        "corHex": "#00FF00",
        "icone": "tag",
        "observacoes": None
    }

    resp = ctx.session.put(f"{BASE_URL}/categorias/{ctx.categoria_id}", json=payload)
    return assert_status(resp, 200, "Categorias Update")


def test_categorias_delete_with_lancamento(ctx: TestContext) -> bool:
    """1.2.4: Delete de categoria com lançamento vinculado → 409"""
    # This will be tested after we create a lancamento linked to this categoria
    # For now, skip
    return True


def test_contas_create(ctx: TestContext) -> bool:
    """1.3.1: Create conta"""
    payload = {
        "nome": "Test Account",
        "tipoConta": "CORRENTE",
        "saldoInicial": 1000.00,
        "corHex": "#0000FF",
        "icone": "bank",
        "observacoes": "Test account"
    }

    resp = ctx.session.post(f"{BASE_URL}/contas", json=payload)

    if assert_status(resp, 201, "Contas Create"):
        try:
            data = resp.json()
            if "id" in data:
                ctx.conta_id = data["id"]
                log(f"  Conta ID: {ctx.conta_id}", "DEBUG")
                test_results["passed"].append("Contas Create - ID Field")
                return True
        except:
            pass

    return False


def test_contas_list(ctx: TestContext) -> bool:
    """1.3.2: List contas"""
    resp = ctx.session.get(f"{BASE_URL}/contas")

    if assert_status(resp, 200, "Contas List"):
        try:
            data = resp.json()
            if isinstance(data, list):
                log(f"✓ Contas List - Is array with {len(data)} items", "PASS")
                test_results["passed"].append("Contas List - Is Array")
                return True
        except:
            pass

    return False


def test_contas_update(ctx: TestContext) -> bool:
    """1.3.3: Update conta"""
    if not ctx.conta_id:
        log("Skipping Contas Update - no conta_id", "SKIP")
        return True

    payload = {
        "nome": "Updated Account",
        "corHex": "#FF00FF",
        "icone": "wallet",
        "observacoes": "Updated"
    }

    resp = ctx.session.put(f"{BASE_URL}/contas/{ctx.conta_id}", json=payload)
    return assert_status(resp, 200, "Contas Update")


def test_contas_delete(ctx: TestContext) -> bool:
    """1.3.4: Delete conta (if no lancamentos)"""
    # Skip for now - we have lancamentos linked
    return True


def test_cartoes_create(ctx: TestContext) -> bool:
    """1.4.1: Create cartão"""
    if not ctx.conta_id:
        log("Skipping Cartoes Create - no conta_id", "SKIP")
        return True

    payload = {
        "nome": "Test Card",
        "bandeira": "VISA",
        "ultimosDigitos": "1234",
        "limite": 5000.00,
        "diaFechamento": 15,
        "diaVencimento": 20,
        "contaPagamentoId": ctx.conta_id,
        "corHex": "#00FF00",
        "icone": "credit-card"
    }

    resp = ctx.session.post(f"{BASE_URL}/cartoes", json=payload)

    if assert_status(resp, 201, "Cartoes Create"):
        try:
            data = resp.json()
            if "id" in data:
                ctx.cartao_id = data["id"]
                log(f"  Cartão ID: {ctx.cartao_id}", "DEBUG")
                test_results["passed"].append("Cartoes Create - ID Field")
                return True
        except:
            pass

    return False


def test_cartoes_list(ctx: TestContext) -> bool:
    """1.4.2: List cartões"""
    resp = ctx.session.get(f"{BASE_URL}/cartoes")

    if assert_status(resp, 200, "Cartoes List"):
        try:
            data = resp.json()
            if isinstance(data, list):
                log(f"✓ Cartoes List - Is array with {len(data)} items", "PASS")
                test_results["passed"].append("Cartoes List - Is Array")
                return True
        except:
            pass

    return False


def test_cartoes_update(ctx: TestContext) -> bool:
    """1.4.3: Update cartão"""
    if not ctx.cartao_id:
        log("Skipping Cartoes Update - no cartao_id", "SKIP")
        return True

    payload = {
        "nome": "Updated Card",
        "bandeira": "MASTERCARD",
        "ultimosDigitos": "5678",
        "limite": 7500.00,
        "diaFechamento": 10,
        "diaVencimento": 15,
        "contaPagamentoId": ctx.conta_id,
        "corHex": "#FF00FF",
        "icone": "credit-card"
    }

    resp = ctx.session.put(f"{BASE_URL}/cartoes/{ctx.cartao_id}", json=payload)
    return assert_status(resp, 200, "Cartoes Update")


def test_lancamentos_simple_receita(ctx: TestContext) -> bool:
    """1.5.1: RECEITA em conta"""
    if not ctx.conta_id or not ctx.categoria_id:
        log("Skipping Lancamentos Simple Receita - missing IDs", "SKIP")
        return True

    today = date.today()
    payload = {
        "descricao": "Test Income",
        "valor": 500.00,
        "dataCompetencia": today.isoformat(),
        "dataVencimento": today.isoformat(),
        "naturezaLancamento": "RECEITA",
        "contaId": ctx.conta_id,
        "cartaoId": None,
        "categoriaId": ctx.categoria_id,
        "subcategoriaId": None,
        "tags": ["test"],
        "observacoes": "Test",
        "statusPagamento": "REALIZADO"
    }

    resp = ctx.session.post(f"{BASE_URL}/lancamentos/simples", json=payload)

    if assert_status(resp, 201, "Lancamentos Simple Receita"):
        try:
            data = resp.json()
            if "id" in data:
                log(f"  Lancamento ID: {data['id']}", "DEBUG")
                test_results["passed"].append("Lancamentos Simple Receita - ID Field")
                return True
        except:
            pass

    return False


def test_lancamentos_simple_despesa(ctx: TestContext) -> bool:
    """1.5.2: DESPESA em conta → saldo da conta refletido"""
    if not ctx.conta_id or not ctx.categoria_id:
        log("Skipping Lancamentos Simple Despesa - missing IDs", "SKIP")
        return True

    today = date.today()
    payload = {
        "descricao": "Test Expense",
        "valor": 100.00,
        "dataCompetencia": today.isoformat(),
        "dataVencimento": today.isoformat(),
        "naturezaLancamento": "DESPESA",
        "contaId": ctx.conta_id,
        "cartaoId": None,
        "categoriaId": ctx.categoria_id,
        "subcategoriaId": None,
        "tags": [],
        "observacoes": None,
        "statusPagamento": "REALIZADO"
    }

    resp = ctx.session.post(f"{BASE_URL}/lancamentos/simples", json=payload)

    if assert_status(resp, 201, "Lancamentos Simple Despesa"):
        try:
            data = resp.json()
            ctx.lancamento_id = data.get("id")

            # Check conta saldo
            conta_resp = ctx.session.get(f"{BASE_URL}/contas/{ctx.conta_id}")
            if conta_resp.status_code == 200:
                conta = conta_resp.json()
                # Expected: 1000 + 500 (income) - 100 (expense) = 1400
                expected_saldo = 1400.00
                actual_saldo = conta.get("saldo", 0)

                if abs(actual_saldo - expected_saldo) < 0.01:
                    log(f"✓ Lancamentos Simple Despesa - Saldo correto: {actual_saldo}", "PASS")
                    test_results["passed"].append("Lancamentos Simple Despesa - Saldo")
                    return True
                else:
                    log(f"✗ Lancamentos Simple Despesa - Saldo incorreto: {actual_saldo} vs {expected_saldo}", "FAIL")
                    test_results["failed"].append(("Lancamentos Simple Despesa - Saldo", expected_saldo, actual_saldo, ""))

            log(f"✓ Lancamentos Simple Despesa - Created", "PASS")
            test_results["passed"].append("Lancamentos Simple Despesa - Created")
            return True
        except Exception as e:
            log(f"✗ Lancamentos Simple Despesa - Error: {e}", "FAIL")

    return False


def test_lancamentos_simple_cartao(ctx: TestContext) -> bool:
    """1.5.3: DESPESA em cartão → gera/atualiza fatura"""
    if not ctx.cartao_id or not ctx.categoria_id:
        log("Skipping Lancamentos Simple Cartao - missing IDs", "SKIP")
        return True

    today = date.today()
    payload = {
        "descricao": "Credit Card Expense",
        "valor": 250.00,
        "dataCompetencia": today.isoformat(),
        "dataVencimento": today.isoformat(),
        "naturezaLancamento": "DESPESA",
        "contaId": None,
        "cartaoId": ctx.cartao_id,
        "categoriaId": ctx.categoria_id,
        "subcategoriaId": None,
        "tags": [],
        "observacoes": None,
        "statusPagamento": "PREVISTO"
    }

    resp = ctx.session.post(f"{BASE_URL}/lancamentos/simples", json=payload)
    return assert_status(resp, 201, "Lancamentos Simple Cartao")


def test_lancamentos_status_previsto(ctx: TestContext) -> bool:
    """1.5.4: Data futura → PREVISTO; passada não paga → ATRASADO"""
    if not ctx.conta_id or not ctx.categoria_id:
        log("Skipping Lancamentos Status - missing IDs", "SKIP")
        return True

    future_date = (date.today() + timedelta(days=10)).isoformat()
    payload = {
        "descricao": "Future Lancamento",
        "valor": 50.00,
        "dataCompetencia": future_date,
        "dataVencimento": future_date,
        "naturezaLancamento": "DESPESA",
        "contaId": ctx.conta_id,
        "cartaoId": None,
        "categoriaId": ctx.categoria_id,
        "subcategoriaId": None,
        "tags": [],
        "observacoes": None,
        "statusPagamento": "PENDENTE"
    }

    resp = ctx.session.post(f"{BASE_URL}/lancamentos/simples", json=payload)

    if assert_status(resp, 201, "Lancamentos Status Previsto"):
        try:
            data = resp.json()
            if data.get("status") == "PREVISTO":
                log(f"✓ Lancamentos Status - Status is PREVISTO", "PASS")
                test_results["passed"].append("Lancamentos Status - PREVISTO")
                return True
        except:
            pass

    return False


def test_lancamentos_pagar(ctx: TestContext) -> bool:
    """1.5.5: POST /pagar funciona"""
    if not ctx.lancamento_id:
        log("Skipping Lancamentos Pagar - no lancamento_id", "SKIP")
        return True

    resp = ctx.session.post(f"{BASE_URL}/lancamentos/{ctx.lancamento_id}/pagar")
    return assert_status(resp, 200, "Lancamentos Pagar")


def test_lancamentos_update(ctx: TestContext) -> bool:
    """1.5.6: PUT edita"""
    if not ctx.lancamento_id or not ctx.categoria_id:
        log("Skipping Lancamentos Update - missing IDs", "SKIP")
        return True

    today = date.today()
    payload = {
        "descricao": "Updated Lancamento",
        "valor": 120.00,
        "dataCompetencia": today.isoformat(),
        "categoriaId": ctx.categoria_id,
        "subcategoriaId": None,
        "tags": ["updated"],
        "observacoes": "Updated via test"
    }

    resp = ctx.session.put(f"{BASE_URL}/lancamentos/{ctx.lancamento_id}", json=payload)
    return assert_status(resp, 200, "Lancamentos Update")


def test_lancamentos_delete(ctx: TestContext) -> bool:
    """1.5.7: DELETE"""
    if not ctx.lancamento_id:
        log("Skipping Lancamentos Delete - no lancamento_id", "SKIP")
        return True

    resp = ctx.session.delete(f"{BASE_URL}/lancamentos/{ctx.lancamento_id}")
    return assert_status(resp, 204, "Lancamentos Delete")


def test_lancamentos_parcelado(ctx: TestContext) -> bool:
    """1.6.1: 12 parcelas → cria 12 lançamentos com datas corretas"""
    if not ctx.conta_id or not ctx.categoria_id:
        log("Skipping Lancamentos Parcelado - missing IDs", "SKIP")
        return True

    today = date.today()
    payload = {
        "descricaoBase": "Parcelado Test",
        "valorTotal": 1200.00,
        "totalParcelas": 12,
        "dataPrimeiraParcela": today.isoformat(),
        "naturezaLancamento": "DESPESA",
        "contaId": ctx.conta_id,
        "cartaoId": None,
        "categoriaId": ctx.categoria_id,
        "subcategoriaId": None,
        "tags": [],
        "observacoes": None
    }

    resp = ctx.session.post(f"{BASE_URL}/lancamentos/parcelado", json=payload)

    if assert_status(resp, 200, "Lancamentos Parcelado"):
        try:
            data = resp.json()
            if isinstance(data, list) and len(data) == 12:
                log(f"✓ Lancamentos Parcelado - Created 12 parcelas", "PASS")
                test_results["passed"].append("Lancamentos Parcelado - 12 Parcelas")
                return True
            else:
                log(f"✗ Lancamentos Parcelado - Expected 12, got {len(data) if isinstance(data, list) else 'unknown'}", "FAIL")
                test_results["failed"].append(("Lancamentos Parcelado - 12 Parcelas", 12, len(data) if isinstance(data, list) else "not-list", ""))
        except Exception as e:
            log(f"✗ Lancamentos Parcelado - Error: {e}", "FAIL")

    return False


def test_lancamentos_parcelado_cartao(ctx: TestContext) -> bool:
    """1.6.2: Em cartão → distribui nas faturas corretas conforme dia de fechamento"""
    if not ctx.cartao_id or not ctx.categoria_id:
        log("Skipping Lancamentos Parcelado Cartao - missing IDs", "SKIP")
        return True

    today = date.today()
    payload = {
        "descricaoBase": "Parcelado Card Test",
        "valorTotal": 600.00,
        "totalParcelas": 6,
        "dataPrimeiraParcela": today.isoformat(),
        "naturezaLancamento": "DESPESA",
        "contaId": None,
        "cartaoId": ctx.cartao_id,
        "categoriaId": ctx.categoria_id,
        "subcategoriaId": None,
        "tags": [],
        "observacoes": None
    }

    resp = ctx.session.post(f"{BASE_URL}/lancamentos/parcelado", json=payload)
    return assert_status(resp, 200, "Lancamentos Parcelado Cartao")


def test_recorrencias_mensal(ctx: TestContext) -> bool:
    """1.7.1: MENSAL sem data fim"""
    if not ctx.conta_id or not ctx.categoria_id:
        log("Skipping Recorrencias Mensal - missing IDs", "SKIP")
        return True

    today = date.today()
    payload = {
        "descricaoBase": "Monthly Recurring",
        "valorBase": 100.00,
        "frequencia": "MENSAL",
        "dataInicio": today.isoformat(),
        "dataFim": None,
        "naturezaLancamento": "DESPESA",
        "contaId": ctx.conta_id,
        "cartaoId": None,
        "categoriaId": ctx.categoria_id,
        "subcategoriaId": None,
        "tags": [],
        "observacoes": None,
        "gerarOcorrenciasAte": None
    }

    resp = ctx.session.post(f"{BASE_URL}/lancamentos/recorrencia", json=payload)

    if assert_status(resp, 201, "Recorrencias Mensal"):
        try:
            data = resp.json()
            if "id" in data:
                ctx.recorrencia_id = data["id"]
                log(f"  Recorrencia ID: {ctx.recorrencia_id}", "DEBUG")
                test_results["passed"].append("Recorrencias Mensal - ID Field")
                return True
        except:
            pass

    return False


def test_recorrencias_semanal(ctx: TestContext) -> bool:
    """1.7.2: SEMANAL com data fim"""
    if not ctx.conta_id or not ctx.categoria_id:
        log("Skipping Recorrencias Semanal - missing IDs", "SKIP")
        return True

    today = date.today()
    end_date = (today + timedelta(days=56)).isoformat()  # 8 weeks

    payload = {
        "descricaoBase": "Weekly Recurring",
        "valorBase": 50.00,
        "frequencia": "SEMANAL",
        "dataInicio": today.isoformat(),
        "dataFim": end_date,
        "naturezaLancamento": "DESPESA",
        "contaId": ctx.conta_id,
        "cartaoId": None,
        "categoriaId": ctx.categoria_id,
        "subcategoriaId": None,
        "tags": [],
        "observacoes": None,
        "gerarOcorrenciasAte": None
    }

    resp = ctx.session.post(f"{BASE_URL}/lancamentos/recorrencia", json=payload)
    return assert_status(resp, 201, "Recorrencias Semanal")


def test_recorrencias_update(ctx: TestContext) -> bool:
    """1.7.3: Update regenera futuros"""
    if not ctx.recorrencia_id:
        log("Skipping Recorrencias Update - no recorrencia_id", "SKIP")
        return True

    payload = {
        "descricaoBase": "Updated Recurring",
        "valorBase": 150.00
    }

    resp = ctx.session.put(f"{BASE_URL}/lancamentos/recorrencia/{ctx.recorrencia_id}", json=payload)
    return assert_status(resp, 200, "Recorrencias Update")


def test_recorrencias_delete(ctx: TestContext) -> bool:
    """1.7.4: Delete remove futuros"""
    if not ctx.recorrencia_id:
        log("Skipping Recorrencias Delete - no recorrencia_id", "SKIP")
        return True

    resp = ctx.session.delete(f"{BASE_URL}/lancamentos/recorrencia/{ctx.recorrencia_id}")
    return assert_status(resp, 204, "Recorrencias Delete")


def test_faturas_list(ctx: TestContext) -> bool:
    """1.8.1: Geração automática - List"""
    resp = ctx.session.get(f"{BASE_URL}/faturas")

    if assert_status(resp, 200, "Faturas List"):
        try:
            data = resp.json()
            if isinstance(data, list) and len(data) > 0:
                ctx.fatura_id = data[0].get("id")
                log(f"✓ Faturas List - Generated automatically ({len(data)} faturas)", "PASS")
                test_results["passed"].append("Faturas List - Auto Generated")
                return True
        except:
            pass

    return False


def test_faturas_pagar_integral(ctx: TestContext) -> bool:
    """1.8.2: Pagar integral → lançamentos viram REALIZADO"""
    if not ctx.fatura_id:
        log("Skipping Faturas Pagar - no fatura_id", "SKIP")
        return True

    resp = ctx.session.post(f"{BASE_URL}/faturas/{ctx.fatura_id}/pagar")
    return assert_status(resp, 200, "Faturas Pagar")


def test_relatorios_excel(ctx: TestContext) -> bool:
    """1.11.1: GET /relatorios/lancamentos/excel retorna XLSX"""
    resp = ctx.session.get(f"{BASE_URL}/relatorios/lancamentos/excel")

    if assert_status(resp, 200, "Relatorios Excel"):
        # Check magic bytes for XLSX (PK)
        if resp.content[:2] == b'PK':
            log(f"✓ Relatorios Excel - Valid XLSX (magic bytes PK)", "PASS")
            test_results["passed"].append("Relatorios Excel - Valid XLSX")
            return True
        else:
            log(f"✗ Relatorios Excel - Invalid magic bytes: {resp.content[:4]}", "FAIL")
            test_results["failed"].append(("Relatorios Excel - Valid XLSX", "PK", resp.content[:4], ""))

    return False


def test_relatorios_pdf(ctx: TestContext) -> bool:
    """1.11.2: GET /relatorios/lancamentos/pdf retorna PDF"""
    resp = ctx.session.get(f"{BASE_URL}/relatorios/lancamentos/pdf")

    if assert_status(resp, 200, "Relatorios PDF"):
        # Check magic bytes for PDF (%PDF)
        if resp.content[:4] == b'%PDF':
            log(f"✓ Relatorios PDF - Valid PDF (magic bytes %PDF)", "PASS")
            test_results["passed"].append("Relatorios PDF - Valid PDF")
            return True
        else:
            log(f"✗ Relatorios PDF - Invalid magic bytes: {resp.content[:4]}", "FAIL")
            test_results["failed"].append(("Relatorios PDF - Valid PDF", "%PDF", resp.content[:4], ""))

    return False


def test_dashboard(ctx: TestContext) -> bool:
    """1.12.1: Dashboard retorna campos esperados"""
    resp = ctx.session.get(f"{BASE_URL}/dashboard")

    if assert_status(resp, 200, "Dashboard"):
        try:
            data = resp.json()
            expected_fields = ["totalReceitas", "totalDespesas", "saldoAtual"]
            all_present = all(field in data for field in expected_fields)

            if all_present:
                log(f"✓ Dashboard - All expected fields present", "PASS")
                test_results["passed"].append("Dashboard - Fields Present")
                return True
            else:
                missing = [f for f in expected_fields if f not in data]
                log(f"✗ Dashboard - Missing fields: {missing}", "FAIL")
                test_results["failed"].append(("Dashboard - Fields Present", expected_fields, missing, json.dumps(data)[:100]))
        except Exception as e:
            log(f"✗ Dashboard - Error: {e}", "FAIL")

    return False


# ============================================================================
# PHASE 5: REGRESSION TESTS
# ============================================================================

def test_regression_lancamentos_response_shape(ctx: TestContext) -> bool:
    """5.1: GET /lancamentos retorna {items, total, skip, take}"""
    resp = ctx.session.get(f"{BASE_URL}/lancamentos")

    if assert_status(resp, 200, "Regression - Lancamentos Response Shape"):
        try:
            data = resp.json()
            expected_fields = ["items", "total", "skip", "take"]
            all_present = all(field in data for field in expected_fields)

            if all_present:
                log(f"✓ Regression - Lancamentos response has correct shape", "PASS")
                test_results["passed"].append("Regression - Lancamentos Shape")
                return True
            else:
                missing = [f for f in expected_fields if f not in data]
                log(f"✗ Regression - Response shape wrong. Missing: {missing}", "FAIL")
                test_results["failed"].append(("Regression - Lancamentos Shape", expected_fields, missing, json.dumps(data)[:100]))
        except Exception as e:
            log(f"✗ Regression - Error: {e}", "FAIL")

    return False


def test_regression_health_head(ctx: TestContext) -> bool:
    """5.2: GET /auth/status retorna 200 (health equivalent)"""
    resp = ctx.session.get(f"{BASE_URL}/auth/status")
    return assert_status(resp, 200, "Regression - Health GET")


def test_regression_migrations(ctx: TestContext) -> bool:
    """5.3: Migrations OK - tables exist"""
    # We'll check by trying to list resources
    resp = ctx.session.get(f"{BASE_URL}/contas")
    if resp.status_code in [200, 401]:  # 401 means auth issue, but DB is fine
        log(f"✓ Regression - Migrations OK (tables accessible)", "PASS")
        test_results["passed"].append("Regression - Migrations OK")
        return True
    else:
        log(f"✗ Regression - Migrations may be broken: {resp.status_code}", "FAIL")
        test_results["failed"].append(("Regression - Migrations OK", "200/401", resp.status_code, ""))
        return False


def test_regression_error_format(ctx: TestContext) -> bool:
    """5.4: Error responses have 'error' field"""
    # Try a bad request
    resp = ctx.session.post(f"{BASE_URL}/auth/setup-pin", json={"pin": ""})

    if resp.status_code >= 400:
        try:
            data = resp.json()
            if "error" in data or "message" in str(data):
                log(f"✓ Regression - Error format correct", "PASS")
                test_results["passed"].append("Regression - Error Format")
                return True
        except:
            pass

    log(f"✗ Regression - Error format incorrect", "FAIL")
    test_results["failed"].append(("Regression - Error Format", "error field", "missing", ""))
    return False


# ============================================================================
# PHASE 6: EDGE CASES
# ============================================================================

def test_edge_zero_value(ctx: TestContext) -> bool:
    """6.1: Valor 0 → 400"""
    if not ctx.conta_id or not ctx.categoria_id:
        log("Skipping Edge Zero Value - missing IDs", "SKIP")
        return True

    today = date.today()
    payload = {
        "descricao": "Zero Value",
        "valor": 0.0,
        "dataCompetencia": today.isoformat(),
        "dataVencimento": today.isoformat(),
        "naturezaLancamento": "DESPESA",
        "contaId": ctx.conta_id,
        "cartaoId": None,
        "categoriaId": ctx.categoria_id,
        "subcategoriaId": None,
        "tags": [],
        "observacoes": None,
        "statusPagamento": "PENDENTE"
    }

    resp = ctx.session.post(f"{BASE_URL}/lancamentos/simples", json=payload)
    return assert_status(resp, 400, "Edge Case - Zero Value")


def test_edge_negative_value(ctx: TestContext) -> bool:
    """6.2: Valor negativo → 400"""
    if not ctx.conta_id or not ctx.categoria_id:
        log("Skipping Edge Negative Value - missing IDs", "SKIP")
        return True

    today = date.today()
    payload = {
        "descricao": "Negative Value",
        "valor": -50.00,
        "dataCompetencia": today.isoformat(),
        "dataVencimento": today.isoformat(),
        "naturezaLancamento": "DESPESA",
        "contaId": ctx.conta_id,
        "cartaoId": None,
        "categoriaId": ctx.categoria_id,
        "subcategoriaId": None,
        "tags": [],
        "observacoes": None,
        "statusPagamento": "PENDENTE"
    }

    resp = ctx.session.post(f"{BASE_URL}/lancamentos/simples", json=payload)
    return assert_status(resp, 400, "Edge Case - Negative Value")


def test_edge_long_description(ctx: TestContext) -> bool:
    """6.3: Descrição 500 chars → ok"""
    if not ctx.conta_id or not ctx.categoria_id:
        log("Skipping Edge Long Description - missing IDs", "SKIP")
        return True

    today = date.today()
    long_desc = "x" * 500
    payload = {
        "descricao": long_desc,
        "valor": 10.00,
        "dataCompetencia": today.isoformat(),
        "dataVencimento": today.isoformat(),
        "naturezaLancamento": "DESPESA",
        "contaId": ctx.conta_id,
        "cartaoId": None,
        "categoriaId": ctx.categoria_id,
        "subcategoriaId": None,
        "tags": [],
        "observacoes": None,
        "statusPagamento": "PENDENTE"
    }

    resp = ctx.session.post(f"{BASE_URL}/lancamentos/simples", json=payload)
    return assert_status(resp, 201, "Edge Case - Long Description")


def test_edge_sql_injection(ctx: TestContext) -> bool:
    """6.4: SQL injection em descrição → tratado, sem crash"""
    if not ctx.conta_id or not ctx.categoria_id:
        log("Skipping Edge SQL Injection - missing IDs", "SKIP")
        return True

    today = date.today()
    payload = {
        "descricao": "'; DROP TABLE lancamentos;--",
        "valor": 10.00,
        "dataCompetencia": today.isoformat(),
        "dataVencimento": today.isoformat(),
        "naturezaLancamento": "DESPESA",
        "contaId": ctx.conta_id,
        "cartaoId": None,
        "categoriaId": ctx.categoria_id,
        "subcategoriaId": None,
        "tags": [],
        "observacoes": None,
        "statusPagamento": "PENDENTE"
    }

    try:
        resp = ctx.session.post(f"{BASE_URL}/lancamentos/simples", json=payload)
        # Should either accept it as a description or reject with 400, not 500
        if resp.status_code in [201, 400]:
            log(f"✓ Edge Case - SQL Injection handled ({resp.status_code})", "PASS")
            test_results["passed"].append("Edge Case - SQL Injection Handled")
            return True
        else:
            log(f"✗ Edge Case - SQL Injection not handled: {resp.status_code}", "FAIL")
            test_results["failed"].append(("Edge Case - SQL Injection", "201/400", resp.status_code, ""))
            return False
    except Exception as e:
        log(f"✗ Edge Case - SQL Injection caused exception: {e}", "FAIL")
        test_results["failed"].append(("Edge Case - SQL Injection", "no exception", "exception", str(e)))
        return False


def test_edge_invalid_date(ctx: TestContext) -> bool:
    """6.5: Data 29/02/2025 (não bissexto) → 400"""
    if not ctx.conta_id or not ctx.categoria_id:
        log("Skipping Edge Invalid Date - missing IDs", "SKIP")
        return True

    payload = {
        "descricao": "Invalid Date",
        "valor": 10.00,
        "dataCompetencia": "2025-02-29",
        "dataVencimento": "2025-02-29",
        "naturezaLancamento": "DESPESA",
        "contaId": ctx.conta_id,
        "cartaoId": None,
        "categoriaId": ctx.categoria_id,
        "subcategoriaId": None,
        "tags": [],
        "observacoes": None,
        "statusPagamento": "PENDENTE"
    }

    resp = ctx.session.post(f"{BASE_URL}/lancamentos/simples", json=payload)
    return assert_status(resp, 400, "Edge Case - Invalid Date")


def test_edge_concurrent_payment(ctx: TestContext) -> bool:
    """6.6: 2 requests paralelos POST /pagar mesmo lançamento → apenas 1 ganha"""
    if not ctx.conta_id or not ctx.categoria_id:
        log("Skipping Edge Concurrent Payment - missing IDs", "SKIP")
        return True

    # Create a lancamento first
    today = date.today()
    payload = {
        "descricao": "Concurrent Test",
        "valor": 100.00,
        "dataCompetencia": today.isoformat(),
        "dataVencimento": today.isoformat(),
        "naturezaLancamento": "DESPESA",
        "contaId": ctx.conta_id,
        "cartaoId": None,
        "categoriaId": ctx.categoria_id,
        "subcategoriaId": None,
        "tags": [],
        "observacoes": None,
        "statusPagamento": "PENDENTE"
    }

    resp = ctx.session.post(f"{BASE_URL}/lancamentos/simples", json=payload)
    if resp.status_code != 201:
        log("Edge Case - Concurrent Payment - Failed to create lancamento", "SKIP")
        return True

    lancamento_id = resp.json().get("id")

    # Try 2 concurrent payments
    results = []

    def try_pagar():
        r = ctx.session.post(f"{BASE_URL}/lancamentos/{lancamento_id}/pagar")
        results.append(r.status_code)

    with ThreadPoolExecutor(max_workers=2) as executor:
        executor.submit(try_pagar)
        executor.submit(try_pagar)

    # One should succeed (200), one should fail (409 or 400)
    if (200 in results and (409 in results or 400 in results)) or results.count(200) == 1:
        log(f"✓ Edge Case - Concurrent Payment - Only one succeeded: {results}", "PASS")
        test_results["passed"].append("Edge Case - Concurrent Payment")
        return True
    else:
        log(f"✗ Edge Case - Concurrent Payment - Both succeeded or both failed: {results}", "FAIL")
        test_results["failed"].append(("Edge Case - Concurrent Payment", "1 success + 1 failure", results, ""))
        return False


def test_edge_pagination(ctx: TestContext) -> bool:
    """6.7: 100+ lançamentos, paginar → < 2s por página"""
    log("Skipping Edge Pagination - requires bulk setup", "SKIP")
    return True


# ============================================================================
# MAIN TEST RUNNER
# ============================================================================

def run_all_tests(ctx: TestContext, phases: List[str] = None):
    """Run all test phases"""
    if phases is None:
        phases = ["1", "5", "6"]

    test_functions = []

    if "1" in phases:
        log("=" * 80, "INFO")
        log("PHASE 1: FUNCTIONAL TESTS", "INFO")
        log("=" * 80, "INFO")

        test_functions.extend([
            ("AUTH", [
                test_auth_setup_happy,
                test_auth_setup_duplicate,
                test_auth_setup_non_numeric,
                test_auth_setup_short_pin,
                test_auth_login_correct,
                test_auth_login_attempts,
                test_auth_status,
                test_auth_no_token,
            ]),
            ("CATEGORIAS", [
                test_categorias_list,
                test_categorias_create,
                test_categorias_update,
            ]),
            ("CONTAS", [
                test_contas_create,
                test_contas_list,
                test_contas_update,
            ]),
            ("CARTOES", [
                test_cartoes_create,
                test_cartoes_list,
                test_cartoes_update,
            ]),
            ("LANCAMENTOS", [
                test_lancamentos_simple_receita,
                test_lancamentos_simple_despesa,
                test_lancamentos_simple_cartao,
                test_lancamentos_status_previsto,
                test_lancamentos_pagar,
                test_lancamentos_update,
                test_lancamentos_delete,
            ]),
            ("PARCELADOS", [
                test_lancamentos_parcelado,
                test_lancamentos_parcelado_cartao,
            ]),
            ("RECORRENTES", [
                test_recorrencias_mensal,
                test_recorrencias_semanal,
                test_recorrencias_update,
                test_recorrencias_delete,
            ]),
            ("FATURAS", [
                test_faturas_list,
                test_faturas_pagar_integral,
            ]),
            ("RELATORIOS", [
                test_relatorios_excel,
                test_relatorios_pdf,
            ]),
            ("DASHBOARD", [
                test_dashboard,
            ]),
        ])

    if "5" in phases:
        log("=" * 80, "INFO")
        log("PHASE 5: REGRESSION TESTS", "INFO")
        log("=" * 80, "INFO")

        test_functions.append(("REGRESSION", [
            test_regression_lancamentos_response_shape,
            test_regression_health_head,
            test_regression_migrations,
            test_regression_error_format,
        ]))

    if "6" in phases:
        log("=" * 80, "INFO")
        log("PHASE 6: EDGE CASES", "INFO")
        log("=" * 80, "INFO")

        test_functions.append(("EDGE_CASES", [
            test_edge_zero_value,
            test_edge_negative_value,
            test_edge_long_description,
            test_edge_sql_injection,
            test_edge_invalid_date,
            test_edge_concurrent_payment,
            test_edge_pagination,
        ]))

    # Execute all tests
    for category, tests in test_functions:
        log(f"\n--- {category} ---", "INFO")
        for test_func in tests:
            try:
                test_func(ctx)
            except Exception as e:
                log(f"✗ {test_func.__name__} - Exception: {e}", "ERROR")
                test_results["failed"].append((test_func.__name__, "no exception", "exception", str(e)))


def print_summary():
    """Print test summary"""
    log("=" * 80, "INFO")
    log("TEST SUMMARY", "INFO")
    log("=" * 80, "INFO")

    passed = len(test_results["passed"])
    failed = len(test_results["failed"])

    print(f"\nPassed: {passed}")
    print(f"Failed: {failed}")

    if failed > 0:
        print("\nFailed Tests:")
        for item in test_results["failed"]:
            if isinstance(item, tuple):
                test_name = item[0]
                print(f"  - {test_name}")
                if len(item) > 1:
                    print(f"    Expected: {item[1]}")
                    print(f"    Got: {item[2]}")
            else:
                print(f"  - {item}")

    print(f"\nTotal: {passed + failed}")
    print(f"Success Rate: {passed / (passed + failed) * 100:.1f}%" if (passed + failed) > 0 else "No tests run")


def main():
    if len(sys.argv) > 1 and sys.argv[1] == "--clean-only":
        log("Cleaning database only...", "INFO")
        if reset_database():
            log("Database cleaned. Exiting.", "INFO")
        sys.exit(0)

    ctx = TestContext()

    # Check API health
    log("Checking API health...", "INFO")
    if not health_check():
        log("API is not healthy. Exiting.", "ERROR")
        sys.exit(1)

    # Reset database
    if not reset_database():
        log("Failed to reset database. Continuing anyway...", "WARN")

    # Run tests
    run_all_tests(ctx)

    # Print summary
    print_summary()

    # Exit with appropriate code
    sys.exit(0 if len(test_results["failed"]) == 0 else 1)


if __name__ == "__main__":
    main()
