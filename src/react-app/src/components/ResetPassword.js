import React, { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { useNavigate } from 'react-router-dom';
import { Spinner } from 'reactstrap';
import { Lock } from 'react-bootstrap-icons';
import { toast } from 'react-toastify';
import ErrorMessage from './ErrorMessage';
import AuthCard from './AuthCard';
import { passwordService } from '../services/passwordService';
import { PASSWORD_RULES } from '../validations';

export default function ResetPassword() {
  const navigate = useNavigate();
  const { register, handleSubmit, watch, formState: { errors, isSubmitting } } = useForm({
    defaultValues: { password: '', confirmPassword: '' }
  });

  const password = watch('password');

  useEffect(() => {
    try {
      const raw = sessionStorage.getItem('resetPassword');
      if (!raw) { navigate('/forgot-password'); return; }
      const parsed = JSON.parse(raw);
      if (!parsed.code) navigate('/reset-password/code');
    } catch {
      navigate('/forgot-password');
    }
  }, [navigate]);

  const onSubmit = async ({ password }) => {
    let email, code;
    try {
      ({ email, code } = JSON.parse(sessionStorage.getItem('resetPassword')));
    } catch {
      navigate('/forgot-password');
      return;
    }

    try {
      await passwordService.reset(email, code, password);
      sessionStorage.removeItem('resetPassword');
      toast.success('Password updated! Please log in');
      navigate('/login');
    } catch {}
  };

  return (
    <AuthCard>
      <form onSubmit={handleSubmit(onSubmit)}>
        <h1>New Password</h1>
        <p className="text-muted">Choose a strong password for your account</p>
        <div className="mb-3 input-group">
          <span className="input-group-text"><Lock /></span>
          <input
            placeholder="New password"
            autoComplete="new-password"
            type="password"
            className="form-control"
            {...register('password', PASSWORD_RULES)}
          />
        </div>
        <ErrorMessage error={errors.password} />
        <div className="mb-4 input-group">
          <span className="input-group-text"><Lock /></span>
          <input
            placeholder="Confirm password"
            autoComplete="new-password"
            type="password"
            className="form-control"
            {...register('confirmPassword', {
              validate: value => value === password || 'The passwords do not match'
            })}
          />
        </div>
        <ErrorMessage error={errors.confirmPassword} />
        <button className="btn btn-success w-100" disabled={isSubmitting}>
          {isSubmitting ? <Spinner size="sm" /> : 'Save New Password'}
        </button>
      </form>
    </AuthCard>
  );
}
