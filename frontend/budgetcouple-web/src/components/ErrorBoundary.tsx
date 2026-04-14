import type { ReactNode, ReactElement } from 'react';
import React from 'react';
import { AlertTriangle } from 'lucide-react';

interface Props {
  children: ReactNode;
}

interface State {
  hasError: boolean;
  error: Error | null;
}

export class ErrorBoundary extends React.Component<Props, State> {
  constructor(props: Props) {
    super(props);
    this.state = {
      hasError: false,
      error: null,
    };
  }

  static getDerivedStateFromError(error: Error): State {
    return {
      hasError: true,
      error,
    };
  }

  componentDidCatch(error: Error) {
    console.error('ErrorBoundary caught an error:', error);
  }

  resetError = () => {
    this.setState({
      hasError: false,
      error: null,
    });
  };

  render(): ReactElement {
    if (this.state.hasError) {
      return (
        <div className="flex items-center justify-center min-h-screen bg-gray-50">
          <div className="max-w-md w-full bg-white rounded-lg shadow-md p-6">
            <div className="flex items-center justify-center mb-4">
              <AlertTriangle className="w-12 h-12 text-red-500" />
            </div>
            <h1 className="text-2xl font-bold text-gray-900 text-center mb-2">
              Algo deu errado
            </h1>
            <p className="text-gray-600 text-center mb-4">
              Desculpe, ocorreu um erro inesperado. Por favor, tente novamente.
            </p>
            {import.meta.env.DEV && this.state.error && (
              <details className="mb-4 text-xs bg-gray-100 p-2 rounded border border-gray-300">
                <summary className="font-semibold text-gray-700 cursor-pointer">
                  Detalhes do erro
                </summary>
                <pre className="mt-2 text-red-600 overflow-auto max-h-32">
                  {this.state.error.toString()}
                </pre>
              </details>
            )}
            <button
              onClick={this.resetError}
              className="w-full bg-blue-600 text-white font-semibold py-2 px-4 rounded hover:bg-blue-700 transition-colors"
            >
              Tentar novamente
            </button>
          </div>
        </div>
      );
    }

    return this.props.children as ReactElement;
  }
}
