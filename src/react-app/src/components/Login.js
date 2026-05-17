import React, { useEffect, useState, useCallback } from 'react';
import { useForm } from 'react-hook-form';
import { Person, Lock } from 'react-bootstrap-icons';
import { Link, useNavigate } from 'react-router-dom';
import { Spinner } from 'reactstrap';
import { toast } from 'react-toastify';
import ErrorMessage from './ErrorMessage';
import AuthCard from './AuthCard';
import { useAuth } from '../AuthContext';
import { authService } from '../services/authService';
import { EMAIL_PATTERN } from '../validations';
import './Login.css';

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
      <form onSubmit={handleSubmit(onSubmit)}>
        <h1>Login</h1>
        <p className="text-muted">Sign In to your account</p>
        <div className="mb-3 input-group">
          <span className="input-group-text"><Person /></span>
          <input
            placeholder="E-mail"
            autoComplete="email"
            type="text"
            className="form-control"
            {...register('email', { required: 'E-mail is required', pattern: EMAIL_PATTERN })}
          />
        </div>
        <ErrorMessage error={errors.email} />
        <div className="mb-4 input-group">
          <span className="input-group-text"><Lock /></span>
          <input
            placeholder="Password"
            autoComplete="current-password"
            type="password"
            className="form-control"
            {...register('password', { required: 'Password is required' })}
          />
        </div>
        <ErrorMessage error={errors.password} />
        {unverifiedEmail && (
          <div className="alert alert-warning py-2 mb-3" role="alert">
            <small>
              Seu e-mail ainda não foi verificado.{' '}
              <button
                type="button"
                className="alert-link btn btn-link p-0 align-baseline"
                onClick={handleVerifyEmail}
                disabled={resending}
              >
                {resending ? <Spinner size="sm" /> : 'Verificar agora'}
              </button>
            </small>
          </div>
        )}
        <div className="row">
          <div className="col-6">
            <button className="px-4 btn btn-primary" disabled={isSubmitting}>
              {isSubmitting ? <Spinner size="sm" /> : 'Login'}
            </button>
          </div>
          <div className="text-end col-6">
            <button type="button" className="px-0 btn btn-link" onClick={() => navigate('/forgot-password')}>Forgot password?</button>
          </div>
        </div>
      </form>
      <hr />
      <p className="text-center text-muted mb-2">Don't have an account?</p>
      <div className="text-center">
        <Link to="/register" className="btn btn-outline-primary w-100">Create account</Link>
      </div>
    </AuthCard>
  );
}
