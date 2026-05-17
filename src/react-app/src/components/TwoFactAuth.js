import React, { useState, useEffect, useCallback } from 'react';
import { useForm } from 'react-hook-form';
import { useNavigate } from 'react-router-dom';
import { Spinner } from 'reactstrap';
import { toast } from 'react-toastify';
import ErrorMessage from './ErrorMessage';
import AuthCard from './AuthCard';
import { useAuth } from '../AuthContext';
import { authService } from '../services/authService';

export default function TwoFactAuth() {
  const navigate = useNavigate();
  const { setAccessToken } = useAuth();
  const [hash, setHash] = useState(null);
  const [sending, setSending] = useState(false);
  const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm({
    defaultValues: { code: '' }
  });

  useEffect(() => {
    const raw = sessionStorage.getItem('tfa');
    if (!raw) { navigate('/login'); return; }
    try {
      const tfa = JSON.parse(raw);
      setHash(tfa.hash);
    } catch {
      navigate('/login');
    }
  }, [navigate]);

  const resendCode = useCallback(async () => {
    if (!hash) return;
    setSending(true);
    try {
      await authService.sendEmailOtp({ hash });
      toast.info('Código reenviado para o seu email');
    } catch {
      toast.error('Erro ao reenviar o código. Tente novamente');
    } finally {
      setSending(false);
    }
  }, [hash]);

  const onSubmit = async ({ code }) => {
    try {
      const { data } = await authService.verifyEmailOtp({ hash, code });
      if (data?.accessToken) {
        sessionStorage.removeItem('tfa');
        setAccessToken(data.accessToken);
        toast.success('Autenticação realizada com sucesso');
        navigate('/');
      }
    } catch {}
  };

  return (
    <AuthCard>
      <form onSubmit={handleSubmit(onSubmit)}>
        <h3>Verificação em Duas Etapas</h3>
        <p className="text-muted mb-1">
          Um código de verificação foi enviado para o seu email.
          Digite-o abaixo para continuar.
        </p>
        <p className="text-muted mb-3" style={{ fontSize: '0.875rem' }}>
          O código expira em 10 minutos.
        </p>
        <div className="mb-3 input-group">
          <input
            type="number"
            className="form-control"
            placeholder="000000"
            {...register('code', {
              required: 'O código é obrigatório',
              minLength: { value: 6, message: 'O código deve ter 6 dígitos' },
              maxLength: { value: 6, message: 'O código deve ter 6 dígitos' }
            })}
          />
        </div>
        <ErrorMessage error={errors.code} />
        <button className="btn btn-success w-100" disabled={isSubmitting || sending}>
          {isSubmitting ? <Spinner size="sm" /> : 'Verificar Código'}
        </button>
        <button
          type="button"
          className="btn btn-outline-secondary w-100 mt-2"
          onClick={resendCode}
          disabled={sending || isSubmitting}
        >
          {sending ? <Spinner size="sm" /> : 'Reenviar código'}
        </button>
        <button
          type="button"
          className="btn btn-link w-100 mt-1"
          onClick={() => navigate(-1)}
          disabled={isSubmitting}
        >
          Voltar
        </button>
      </form>
    </AuthCard>
  );
}
