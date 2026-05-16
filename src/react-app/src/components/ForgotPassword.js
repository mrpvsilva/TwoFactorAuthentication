import React from 'react';
import { useForm } from 'react-hook-form';
import { useNavigate } from 'react-router-dom';
import { Spinner } from 'reactstrap';
import { toast } from 'react-toastify';
import ErrorMessage from './ErrorMessage';
import AuthCard from './AuthCard';
import { passwordService } from '../services/passwordService';
import { EMAIL_PATTERN } from '../validations';

export default function ForgotPassword() {
  const navigate = useNavigate();
  const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm({
    defaultValues: { email: '' }
  });

  const onSubmit = async ({ email }) => {
    try {
      await passwordService.forgot(email);
      sessionStorage.setItem('resetPassword', JSON.stringify({ email }));
      toast.success('Code sent! Check your inbox');
      navigate('/reset-password/code');
    } catch {}
  };

  return (
    <AuthCard>
      <form onSubmit={handleSubmit(onSubmit)}>
        <h1>Forgot Password</h1>
        <p className="text-muted">Enter your e-mail to receive a reset code</p>
        <div className="mb-3 input-group">
          <input
            placeholder="E-mail"
            autoComplete="email"
            type="text"
            className="form-control"
            {...register('email', { required: 'E-mail is required', pattern: EMAIL_PATTERN })}
          />
        </div>
        <ErrorMessage error={errors.email} />
        <button className="btn btn-primary w-100" disabled={isSubmitting}>
          {isSubmitting ? <Spinner size="sm" /> : 'Send Code'}
        </button>
        <button
          type="button"
          className="btn btn-outline-secondary w-100 mt-2"
          onClick={() => navigate('/login')}
        >
          Back to Login
        </button>
      </form>
    </AuthCard>
  );
}
