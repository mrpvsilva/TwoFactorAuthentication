import React, { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { useNavigate } from 'react-router-dom';
import { Container, Spinner } from 'reactstrap';
import ErrorMessage from './ErrorMessage';
import Api from '../api';

export default function ResetPasswordCode() {
  const navigate = useNavigate();
  const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm({
    defaultValues: { code: '' }
  });

  useEffect(() => {
    if (!sessionStorage.getItem('resetPassword')) navigate('/forgot-password');
  }, [navigate]);

  const onSubmit = async ({ code }) => {
    const { email } = JSON.parse(sessionStorage.getItem('resetPassword'));
    await Api.post('/password/verify-code', { email, code })
      .then(() => {
        sessionStorage.setItem('resetPassword', JSON.stringify({ email, code }));
        navigate('/reset-password/new');
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
                <h1>Enter Code</h1>
                <p className="text-muted">Enter the 6-digit code sent to your e-mail</p>
                <div className="mb-3">
                  <input
                    type="text"
                    inputMode="numeric"
                    maxLength={6}
                    placeholder="000000"
                    className="form-control form-control-lg text-center"
                    style={{ letterSpacing: '0.5em', fontWeight: 'bold' }}
                    {...register('code', {
                      required: 'Code is required',
                      pattern: {
                        value: /^[0-9]{6}$/,
                        message: 'Code must be exactly 6 digits'
                      }
                    })}
                  />
                </div>
                <ErrorMessage error={errors.code} />
                <button className="btn btn-primary w-100" disabled={isSubmitting}>
                  {isSubmitting ? <Spinner size="sm" /> : 'Verify Code'}
                </button>
                <button
                  type="button"
                  className="btn btn-outline-secondary w-100 mt-2"
                  onClick={() => navigate('/forgot-password')}
                >
                  Resend Code
                </button>
              </form>
            </div>
          </div>
        </div>
      </div>
    </Container>
  );
}
