import i18n from 'i18next'
import { initReactI18next } from 'react-i18next'

void i18n.use(initReactI18next).init({
  resources: {
    'pt-BR': {
      translation: {
        app: { name: 'BudgetCouple' },
        auth: {
          title: {
            setup: 'Configure seu PIN',
            login: 'Entrar',
            changePin: 'Alterar PIN',
          },
          pin: 'PIN',
          pinConfirm: 'Confirmar PIN',
          pinCurrent: 'PIN Atual',
          pinNew: 'Novo PIN',
          submit: {
            setup: 'Configurar PIN',
            login: 'Entrar',
            change: 'Alterar PIN',
          },
          errors: {
            mismatch: 'Os PINs não conferem',
            invalidPin: 'PIN incorreto',
            locked: 'Acesso bloqueado',
          },
        },
      },
    },
  },
  lng: 'pt-BR',
  fallbackLng: 'pt-BR',
  interpolation: { escapeValue: false },
})

export default i18n
