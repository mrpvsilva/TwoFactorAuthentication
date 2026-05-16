import React from 'react';
import { useForm } from 'react-hook-form';
import { Lock } from 'react-bootstrap-icons';
import { useNavigate } from 'react-router-dom';
import { Container } from 'reactstrap';
import { toast } from 'react-toastify';
import ErrorMessage from './ErrorMessage';
import Api from '../api';

export default function Register() {
  const navigate = useNavigate();
  const { register, handleSubmit, watch, formState: { errors } } = useForm({
    defaultValues: { email: '', password: '', repeatpassword: '' }
  });

  const password = watch('password', '');

  const onSubmit = data => {
    Api.post('/account', data)
      .then(({ data }) => {
        if (data) {
          navigate('/login');
          toast.success('Success');
        }
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
                <h1>Register</h1>
                <p className="text-muted">Create your account</p>
                <div className="mb-3 input-group">
                  <span className="input-group-text">@</span>
                  <input
                    placeholder="Email"
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
                <button className="btn btn-success w-100">Create Account</button>
                <button type="button" className="btn btn-outline-secondary w-100 mt-2" onClick={() => navigate(-1)}>Back</button>
              </form>
            </div>
          </div>
        </div>
      </div>
    </Container>
  );
}
