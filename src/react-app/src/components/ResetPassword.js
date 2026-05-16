import React, { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { useNavigate } from 'react-router-dom';
import { Container } from 'reactstrap';
import { Lock } from 'react-bootstrap-icons';
import { toast } from 'react-toastify';
import ErrorMessage from './ErrorMessage';
import Api from '../api';

export default function ResetPassword() {
  const navigate = useNavigate();
  const { register, handleSubmit, watch, formState: { errors, isSubmitting } } = useForm({
    defaultValues: { password: '', confirmPassword: '' }
  });

  const password = watch('password');

  useEffect(() => {
    const data = sessionStorage.getItem('resetPassword');
    if (!data) { navigate('/forgot-password'); return; }
    const parsed = JSON.parse(data);
    if (!parsed.code) navigate('/reset-password/code');
  }, [navigate]);

  const onSubmit = async ({ password }) => {
    const { email, code } = JSON.parse(sessionStorage.getItem('resetPassword'));
    await Api.post('/password/reset', { email, code, password })
      .then(() => {
        sessionStorage.removeItem('resetPassword');
        toast.success('Password updated! Please log in');
        navigate('/login');
      })
      .catch(() => {});
  };

  return (
    <Container style={{ marginTop: '10%' }}>
      <div className="justify-content-center row">
        <div className="col-md-5 col-lg-4">
          <div className="card">
            <div className="p-4 card-body">
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
                    {...register('password', {
                      required: 'Password is required',
                      minLength: { value: 8, message: 'Password must be at least 8 characters' },
                      pattern: {
                        value: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d])/,
                        message: 'Password must contain uppercase, lowercase, number and special character'
                      }
                    })}
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
                  {isSubmitting ? 'Saving...' : 'Save New Password'}
                </button>
              </form>
            </div>
          </div>
        </div>
      </div>
    </Container>
  );
}
