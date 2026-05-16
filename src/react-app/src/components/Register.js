import React from 'react';
import { useForm } from 'react-hook-form';
import { Lock } from 'react-bootstrap-icons';
import { useNavigate } from 'react-router-dom';
import { Spinner } from 'reactstrap';
import { toast } from 'react-toastify';
import ErrorMessage from './ErrorMessage';
import AuthCard from './AuthCard';
import { authService } from '../services/authService';
import { EMAIL_PATTERN } from '../validations';

export default function Register() {
  const navigate = useNavigate();
  const { register, handleSubmit, watch, formState: { errors, isSubmitting } } = useForm({
    defaultValues: { email: '', password: '', repeatpassword: '' }
  });

  const password = watch('password', '');

  const onSubmit = async (data) => {
    try {
      await authService.register(data);
      toast.success('Success');
      navigate('/login');
    } catch {}
  };

  return (
    <AuthCard>
      <form onSubmit={handleSubmit(onSubmit)}>
        <h1>Register</h1>
        <p className="text-muted">Create your account</p>
        <div className="mb-3 input-group">
          <span className="input-group-text">@</span>
          <input
            placeholder="Email"
            autoComplete="email"
            type="text"
            className="form-control"
            {...register('email', { required: 'E-mail is required', pattern: EMAIL_PATTERN })}
          />
        </div>
        <ErrorMessage error={errors.email} />
        <div className="mb-3 input-group">
          <span className="input-group-text"><Lock /></span>
          <input
            placeholder="Password"
            autoComplete="new-password"
            type="password"
            className="form-control"
            {...register('password', {
              required: 'Password is required',
              minLength: { value: 8, message: 'Password must have at least 8 characters' }
            })}
          />
        </div>
        <ErrorMessage error={errors.password} />
        <div className="mb-4 input-group">
          <span className="input-group-text"><Lock /></span>
          <input
            placeholder="Repeat password"
            autoComplete="new-password"
            type="password"
            className="form-control"
            {...register('repeatpassword', {
              validate: value => value === password || 'The passwords do not match'
            })}
          />
        </div>
        <ErrorMessage error={errors.repeatpassword} />
        <button className="btn btn-success w-100" disabled={isSubmitting}>
          {isSubmitting ? <Spinner size="sm" /> : 'Create Account'}
        </button>
        <button type="button" className="btn btn-outline-secondary w-100 mt-2" onClick={() => navigate(-1)} disabled={isSubmitting}>Back</button>
      </form>
    </AuthCard>
  );
}
