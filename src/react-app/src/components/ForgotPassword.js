import React from 'react';
import { useForm } from 'react-hook-form';
import { useNavigate } from 'react-router-dom';
import { Container } from 'reactstrap';
import { toast } from 'react-toastify';
import ErrorMessage from './ErrorMessage';
import Api from '../api';

export default function ForgotPassword() {
  const navigate = useNavigate();
  const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm({
    defaultValues: { email: '' }
  });

  const onSubmit = async ({ email }) => {
    await Api.post('/password/forgot', { email })
      .then(() => {
        sessionStorage.setItem('resetPassword', JSON.stringify({ email }));
        toast.success('Code sent! Check your inbox');
        navigate('/reset-password/code');
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
                <h1>Forgot Password</h1>
                <p className="text-muted">Enter your e-mail to receive a reset code</p>
                <div className="mb-3 input-group">
                  <input
                    placeholder="E-mail"
                    autoComplete="email"
                    type="text"
                    className="form-control"
                    {...register('email', {
                      required: 'E-mail is required',
                      pattern: {
                        value: /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$/i,
                        message: 'E-mail is invalid'
                      }
                    })}
                  />
                </div>
                <ErrorMessage error={errors.email} />
                <button className="btn btn-primary w-100" disabled={isSubmitting}>
                  {isSubmitting ? 'Sending...' : 'Send Code'}
                </button>
                <button
                  type="button"
                  className="btn btn-outline-secondary w-100 mt-2"
                  onClick={() => navigate('/login')}
                >
                  Back to Login
                </button>
              </form>
            </div>
          </div>
        </div>
      </div>
    </Container>
  );
}
