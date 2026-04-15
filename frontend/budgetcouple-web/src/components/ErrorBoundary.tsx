import type { ReactNode, ReactElement } from 'react';
import React from 'react';
import { AlertTriangle, Copy, Home, FileText } from 'lucide-react';
import { useNavigate } from 'react-router-dom';

interface Props {
  children: ReactNode;
}

interface State {
  hasError: boolean;
  error: Error | null;
  errorInfo: React.ErrorInfo | null;
  componentName?: string;
}

export class ErrorBoundary extends React.Component<Props, State> {
  constructor(props: Props) {
    super(props);
    this.state = {
      hasError: false,
      error: null,
      errorInfo: null,
      componentName: undefined,
    };
  }

  static getDerivedStateFromError(error: Error): Omit<State, 'errorInfo' | 'componentName'> {
    return {
      hasError: true,
      error,
    };
  }

  componentDidCatch(error: Error, errorInfo: React.ErrorInfo) {
    // Extract component name from stack
    const componentMatch = errorInfo.componentStack?.match(/in (\w+)/);
    const componentName = componentMatch?.[1] || 'Unknown';

    this.setState({
      errorInfo,
      componentName,
    });

    console.error('ErrorBoundary caught an error:', error);
    console.error('Stack:', error.stack);
    console.error('Component stack:', errorInfo.componentStack);
    console.error('Failed component:', componentName);
  }

  resetError = () => {
    this.setState({
      hasError: false,
      error: null,
      errorInfo: null,
      componentName: undefined,
    });
  };

  copyErrorToClipboard = async () => {
    const errorText = this.state.error?.toString() || 'Unknown error';
    const stack = this.state.error?.stack || '';
    const componentName = this.state.componentName || 'Unknown';
    const fullError = `Component: ${componentName}\n\nError: ${errorText}\n\nStack:\n${stack}`;
    
    try {
      await navigator.clipboard.writeText(fullError);
      alert('Erro copiado! Cole isso em um ticket de suporte.');
    } catch {
      alert('Erro ao copiar para a área de transferência');
    }
  };

  render(): ReactElement {
    if (this.state.hasError) {
      return (
        <ErrorFallback
          error={this.state.error}
          componentName={this.state.componentName}
          onReset={this.resetError}
          onCopy={this.copyErrorToClipboard}
        />
      );
    }

    return this.props.children as ReactElement;
  }
}

interface ErrorFallbackProps {
  error: Error | null;
  componentName?: string;
  onReset: () => void;
  onCopy: () => void;
}

function ErrorFallback({ error, componentName, onReset, onCopy }: ErrorFallbackProps) {
  const navigate = useNavigate();

  return (
    <div className="flex items-center justify-center min-h-screen bg-gray-50">
      <div className="max-w-md w-full bg-white rounded-lg shadow-md p-6">
        <div className="flex items-center justify-center mb-4">
          <AlertTriangle className="w-12 h-12 text-red-500" />
        </div>
        
        <h1 className="text-2xl font-bold text-gray-900 text-center mb-2">
          Algo deu errado
        </h1>
        
        {componentName && (
          <p className="text-sm text-gray-500 text-center mb-4">
            Falha no componente: <code className="bg-gray-100 px-2 py-1 rounded">{componentName}</code>
          </p>
        )}
        
        <p className="text-gray-600 text-center mb-6">
          Desculpe, ocorreu um erro inesperado. Tente novamente ou volte à página inicial.
        </p>

        {error && (
          <details className="mb-6 text-xs bg-gray-100 p-3 rounded border border-gray-300">
            <summary className="font-semibold text-gray-700 cursor-pointer mb-2">
              Detalhes do erro
            </summary>
            <pre className="text-red-600 overflow-auto max-h-40 whitespace-pre-wrap break-words">
              {error.toString()}
            </pre>
          </details>
        )}

        <div className="space-y-2 mb-6">
          <button
            onClick={onCopy}
            className="w-full flex items-center justify-center gap-2 bg-gray-600 text-white font-semibold py-2 px-4 rounded hover:bg-gray-700 transition-colors text-sm"
          >
            <Copy className="w-4 h-4" />
            Copiar erro
          </button>
        </div>

        <div className="space-y-2">
          <button
            onClick={onReset}
            className="w-full bg-blue-600 text-white font-semibold py-2 px-4 rounded hover:bg-blue-700 transition-colors"
          >
            Tentar novamente
          </button>
          
          <button
            onClick={() => navigate('/')}
            className="w-full flex items-center justify-center gap-2 bg-slate-600 text-white font-semibold py-2 px-4 rounded hover:bg-slate-700 transition-colors"
          >
            <Home className="w-4 h-4" />
            Ir para Home
          </button>

          <button
            onClick={() => navigate('/lancamentos')}
            className="w-full flex items-center justify-center gap-2 bg-slate-500 text-white font-semibold py-2 px-4 rounded hover:bg-slate-600 transition-colors text-sm"
          >
            <FileText className="w-4 h-4" />
            Ir para Lançamentos
          </button>
        </div>
      </div>
    </div>
  );
}
