import React, { useRef } from 'react';
import { useForm } from 'react-hook-form';
import { Lock } from 'react-bootstrap-icons';
import ErrorMessage from './ErrorMessage';
import Api from '../api';
import { Container } from 'reactstrap';

export default function Register({ history }) {

    const { register, handleSubmit, watch, errors } = useForm({
        defaultValues: {
            email: '',
            password: '',
            repeatpassword: ''
        }
    });

    const password = useRef({});
    password.current = watch("password", "");

    const onSubmit = data => {

        Api
            .post('/account', data)
            .then(res => res.data && history.push('/login'));
    }

    return (
        <Container style={{ marginTop: '10%' }}>
            <div className="justify-content-center row">
                <div className="col-md-9 col-lg-7 col-xl-6">
                    <div className="mx-4 card">
                        <div className="p-4 card-body">
                            <form onSubmit={handleSubmit(onSubmit)}>
                                <h1>Register</h1>
                                <p className="text-muted">Create your account</p>
                                <div className="mb-3 input-group">
                                    <div className="input-group-prepend">
                                        <span className="input-group-text">@</span>
                                    </div>
                                    <input name="email" placeholder="Email" autoComplete="email" type="text" className="form-control"
                                        ref={register({
                                            required: 'E-mail is required',
                                            pattern: {
                                                value: /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$/i,
                                                message: "E-mail is invalid"
                                            }
                                        })} />
                                </div>
                                <ErrorMessage error={errors.email} />
                                <div className="mb-3 input-group">
                                    <div className="input-group-prepend">
                                        <span className="input-group-text">
                                            <Lock />
                                        </span>
                                    </div>
                                    <input name="password" placeholder="Password" autoComplete="new-password" type="password" className="form-control"
                                        ref={register({
                                            required: 'Password is required',
                                            minLength: {
                                                value: 8,
                                                message: "Password must have at least 8 characters"
                                            }
                                        })} />
                                </div>
                                <ErrorMessage error={errors.password} />
                                <div className="mb-4 input-group">
                                    <div className="input-group-prepend">
                                        <span className="input-group-text">
                                            <Lock />
                                        </span>
                                    </div>
                                    <input name="repeatpassword" placeholder="Repeat password" autoComplete="new-password" type="password" className="form-control"
                                        ref={register({
                                            validate: value =>
                                                value === password.current || "The passwords do not match"
                                        })} />
                                </div>
                                <ErrorMessage error={errors.repeatpassword} />
                                <button className="btn btn-success btn-block">Create Account</button>
                            </form>
                        </div>
                    </div>
                </div>
            </div>
        </Container>
    )
}
