import React, { useEffect, useState, useCallback } from 'react';
import { useForm } from 'react-hook-form';
import { User, Lock, Loader2 } from 'lucide-react';
import { Link, useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import ErrorMessage from './ErrorMessage';
import AuthCard from './AuthCard';
import { useAuth } from '../AuthContext';
import { authService } from '../services/authService';
import { EMAIL_PATTERN } from '../validations';
import { Button } from 'src/components/ui/button';
import { Input } from 'src/components/ui/input';

const UNVERIFIED_MSG = 'não verificado';

export default function Login() {
  const navigate = useNavigate();
  const { accessToken } = useAuth();
  const [unverifiedEmail, setUnverifiedEmail] = useState(null);
  const [resending, setResending] = useState(false);
  const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm({
    defaultValues: { email: '', password: '' }
  });

  useEffect(() => {
    if (accessToken) navigate('/');
  }, [accessToken, navigate]);

  const onSubmit = async (data) => {
    setUnverifiedEmail(null);
    try {
      const { data: tfa } = await authService.login(data);
      sessionStorage.setItem('tfa', JSON.stringify(tfa));
      toast.success('Success');
      navigate('/twofactauth');
    } catch (error) {
      const apiErrors = error.response?.data;
      const isUnverified = Array.isArray(apiErrors) &&
        apiErrors.some(e => e.message?.toLowerCase().includes(UNVERIFIED_MSG));
      if (isUnverified) setUnverifiedEmail(data.email);
    }
  };

  const handleVerifyEmail = useCallback(async () => {
    if (!unverifiedEmail) return;
    setResending(true);
    try {
      const { data } = await authService.resendVerification({ email: unverifiedEmail });
      sessionStorage.setItem('registration', JSON.stringify({ hash: data.hash }));
      toast.info('Código de verificação enviado para o seu e-mail.');
      navigate('/verify-registration');
    } catch {
      toast.error('Erro ao reenviar o código. Tente novamente.');
    } finally {
      setResending(false);
    }
  }, [unverifiedEmail, navigate]);

  return (
    <AuthCard>
      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <div>
          <h1 className="text-2xl font-bold">Login</h1>
          <p className="text-sm text-muted-foreground">Sign In to your account</p>
        </div>

        <div className="space-y-1">
          <div className="relative">
            <User className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
            <Input
              placeholder="E-mail"
              autoComplete="email"
              type="text"
              className="pl-9 text-base"
              {...register('email', { required: 'E-mail is required', pattern: EMAIL_PATTERN })}
            />
          </div>
          <ErrorMessage error={errors.email} />
        </div>

        <div className="space-y-1">
          <div className="relative">
            <Lock className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
            <Input
              placeholder="Password"
              autoComplete="current-password"
              type="password"
              className="pl-9 text-base"
              {...register('password', { required: 'Password is required' })}
            />
          </div>
          <ErrorMessage error={errors.password} />
        </div>

        {unverifiedEmail && (
          <div className="rounded-md bg-yellow-50 border border-yellow-200 px-3 py-2 text-sm text-yellow-800">
            Seu e-mail ainda não foi verificado.{' '}
            <button
              type="button"
              className="font-semibold underline disabled:opacity-50"
              onClick={handleVerifyEmail}
              disabled={resending}
            >
              {resending ? <Loader2 className="inline h-3 w-3 animate-spin" /> : 'Verificar agora'}
            </button>
          </div>
        )}

        <div className="flex items-center justify-between gap-2">
          <Button type="submit" className="flex-1" disabled={isSubmitting}>
            {isSubmitting ? <Loader2 className="h-4 w-4 animate-spin" /> : 'Login'}
          </Button>
          <Button type="button" variant="link" className="px-0 shrink-0" onClick={() => navigate('/forgot-password')}>
            Forgot password?
          </Button>
        </div>

        <hr className="border-border" />
        <p className="text-center text-sm text-muted-foreground">Don't have an account?</p>
        <Button variant="outline" className="w-full" asChild>
          <Link to="/register">Create account</Link>
        </Button>
      </form>
    </AuthCard>
  );
}
