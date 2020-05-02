import React, { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { Person, Lock } from 'react-bootstrap-icons';
import { Link } from 'react-router-dom';
import ErrorMessage from './ErrorMessage';
import Api from '../api';
import { Container } from 'reactstrap';


export default function Login({ history }) {

    const { register, handleSubmit, errors } = useForm({
        defaultValues: {
            email: '',
            password: ''
        }
    });

    const onSubmit = data => {

        Api
            .post('/auth', data)
            .then(({ data }) => {
                if (data) {
                    sessionStorage.tfa = JSON.stringify(data);
                    history.push('/twofactauth');
                }
            });
    }

    useEffect(() => {
        const token = localStorage.getItem('token');
        if (token)
            history.push('/');

    }, [history])

    return (
        <Container style={{ marginTop: '10%' }}>
            <div className="justify-content-center row">
                <div className="col-md-5">
                    <div className="card-group">
                        <div className="p-4 card">
                            <div className="card-body">
                                <form onSubmit={handleSubmit(onSubmit)}>
                                    <h1>Login</h1>
                                    <p className="text-muted">Sign In to your account</p>
                                    <div className="mb-3 input-group">
                                        <div className="input-group-prepend">
                                            <span className="input-group-text">
                                                <Person />
                                            </span>
                                        </div>
                                        <input name="email" placeholder="E-mail" autoComplete="email" type="text" className="form-control"
                                            ref={register({
                                                required: 'E-mail is required',
                                                pattern: {
                                                    value: /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$/i,
                                                    message: "E-mail is invalid"
                                                }
                                            })}
                                        />
                                    </div>
                                    <ErrorMessage error={errors.email} />
                                    <div className="mb-4 input-group">
                                        <div className="input-group-prepend">
                                            <span className="input-group-text">
                                                <Lock />
                                            </span>
                                        </div>
                                        <input name="password" placeholder="Password" autoComplete="current-password" type="password" className="form-control"
                                            ref={register({
                                                required: 'Password is required'
                                            })}
                                        />
                                    </div>
                                    <ErrorMessage error={errors.password} />
                                    <div className="row">
                                        <div className="col-6">
                                            <button className="px-4 btn btn-primary">Login</button>
                                        </div>
                                        <div className="text-right col-6">
                                            <button className="px-0 btn btn-link">Forgot password?</button>
                                        </div>
                                    </div>
                                    <div className="row">
                                        <div className="col-12">
                                            <Link to="register">
                                                <button tabIndex="-1" className="mt-3 btn btn-primary active">Register Now!</button>
                                            </Link>
                                        </div>
                                    </div>
                                </form>
                            </div>
                        </div>
                    </div>
                </div>
            </div>
        </Container>
    )
}
