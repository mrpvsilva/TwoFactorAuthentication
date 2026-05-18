import React, { useState, useEffect, useCallback } from 'react';
import { useForm } from 'react-hook-form';
import { useNavigate } from 'react-router-dom';
import { Loader2 } from 'lucide-react';
import { toast } from 'react-toastify';
import ErrorMessage from './ErrorMessage';
import AuthCard from './AuthCard';
import { authService } from '../services/authService';
import { Button } from 'src/components/ui/button';
import { Input } from 'src/components/ui/input';

export default function VerifyRegistration() {
  const navigate = useNavigate();
  const [hash, setHash] = useState(null);
  const [sending, setSending] = useState(false);
  const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm({
    defaultValues: { code: '' }
  });

  useEffect(() => {
    const raw = sessionStorage.getItem('registration');
    if (!raw) { navigate('/register'); return; }
    try {
      const data = JSON.parse(raw);
      setHash(data.hash);
    } catch {
      navigate('/register');
    }
  }, [navigate]);

  const resendCode = useCallback(async () => {
    if (!hash) return;
    setSending(true);
    try {
      await authService.resendRegistrationOtp({ hash });
      toast.info('Código reenviado para o seu email');
    } catch {
      toast.error('Erro ao reenviar o código. Tente novamente');
    } finally {
      setSending(false);
    }
  }, [hash]);

  const onSubmit = async ({ code }) => {
    try {
      await authService.verifyRegistrationOtp({ hash, code });
      sessionStorage.removeItem('registration');
      toast.success('E-mail verificado com sucesso! Faça login para continuar.');
      navigate('/login');
    } catch {}
  };

  return (
    <AuthCard>
      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <div>
          <h2 className="text-xl font-bold">Verificar E-mail</h2>
          <p className="text-sm text-muted-foreground mt-1">
            Um código de verificação foi enviado para o seu e-mail.
            Digite-o abaixo para concluir o cadastro.
          </p>
          <p className="text-xs text-muted-foreground mt-1">O código expira em 10 minutos.</p>
        </div>

        <div className="space-y-1">
          <Input
            type="number"
            placeholder="000000"
            className="text-base text-center tracking-widest font-bold text-lg"
            {...register('code', {
              required: 'O código é obrigatório',
              minLength: { value: 6, message: 'O código deve ter 6 dígitos' },
              maxLength: { value: 6, message: 'O código deve ter 6 dígitos' }
            })}
          />
          <ErrorMessage error={errors.code} />
        </div>

        <Button type="submit" className="w-full" disabled={isSubmitting || sending}>
          {isSubmitting ? <Loader2 className="h-4 w-4 animate-spin" /> : 'Verificar E-mail'}
        </Button>
        <Button type="button" variant="outline" className="w-full" onClick={resendCode} disabled={sending || isSubmitting}>
          {sending ? <Loader2 className="h-4 w-4 animate-spin" /> : 'Reenviar código'}
        </Button>
        <Button type="button" variant="ghost" className="w-full" onClick={() => navigate('/register')} disabled={isSubmitting}>
          Voltar
        </Button>
      </form>
    </AuthCard>
  );
}
