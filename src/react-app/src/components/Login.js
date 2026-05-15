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
      .catch(() => toast.error('Request error'));
  };

  return (
    <Container style={{ marginTop: '10%' }}>
      <div className="justify-content-center row">
        <div className="col-md-8">
          <div className="card-group">
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
                      <button type="button" className="px-0 btn btn-link">Forgot password?</button>
                    </div>
                  </div>
                </form>
              </div>
            </div>
            <div className="text-white bg-primary py-5 d-none d-md-block card" style={{ width: '44%' }}>
              <div className="text-center card-body">
                <h2>Sign up</h2>
                <p>Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua.</p>
                <Link to='/register'>
                  <button tabIndex="-1" className="mt-3 btn btn-primary active">Register Now!</button>
                </Link>
              </div>
            </div>
          </div>
        </div>
      </div>
    </Container>
  );
}
