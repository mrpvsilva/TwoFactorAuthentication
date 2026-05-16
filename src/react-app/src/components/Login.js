import React, { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { Person, Lock } from 'react-bootstrap-icons';
import { Link, useNavigate } from 'react-router-dom';
import { Container } from 'reactstrap';
import { toast } from 'react-toastify';
import ErrorMessage from './ErrorMessage';
import Api from '../api';
import './Login.css';

export default function Login() {

  const navigate = useNavigate();
  const { register, handleSubmit, formState: { errors } } = useForm({
    defaultValues: { email: '', password: '' }
  });

  useEffect(() => {
    if (localStorage.getItem('token')) navigate('/');
  }, [navigate]);

  const onSubmit = data => {
    Api.post('/auth', data)
      .then(({ data }) => {
        if (data) {
          sessionStorage.tfa = JSON.stringify(data);
          navigate('/twofactauth');
          toast.success('Success');
        }
      })
      .catch(() => {});
  };

  return (
    <Container style={{ marginTop: '10%' }}>
      <div className="justify-content-center row">
        <div className="col-md-5 col-lg-4">
          <div className="p-4 card">
            <div className="card-body">
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
                <div className="row">
                  <div className="col-6">
                    <button className="px-4 btn btn-primary">Login</button>
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
            </div>
          </div>
        </div>
      </div>
    </Container>
  );
}
