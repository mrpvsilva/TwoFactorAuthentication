import React, { useState, useEffect } from 'react'
import QrCode from 'qrcode.react';
import { useForm } from 'react-hook-form';
import ErrorMessage from './ErrorMessage';
import Api from '../api';

export default function TwoFactAuth({ history }) {


    const [tfa, setTfa] = useState({
        authenticatorUri: '',
        hash: '',
        hasTwoFactorAuth: ''
    });

    const { register, handleSubmit, errors } = useForm({
        defaultValues: {
            code: ''
        }
    });

    const onSubmit = data => {
        const { hasTwoFactorAuth } = tfa;
        Api
            .post(`/auth/${hasTwoFactorAuth ? 'VerifyCode' : 'AddTwoFactAuth'}`, { ...data, ...tfa })
            .then(({ data }) => {

                if (data) {
                    sessionStorage.removeItem('tfa');
                    history.push('/');
                }
            })
    }

    useEffect(() => {
        const tfa = sessionStorage.getItem('tfa');

        if (!tfa) {
            history.push('/login');
            return;
        }

        setTfa(JSON.parse(tfa));

    }, [history])

    return (
        <div className="justify-content-center row">
            <div className="col-md-9 col-lg-7 col-xl-6">
                <div className="mx-4 card">
                    <div className="p-4 card-body">
                        <form onSubmit={handleSubmit(onSubmit)}>
                            <h3>Two Factor Authentication</h3>
                            <div className="mb-3 input-group justify-content-center row">
                                {tfa.authenticatorUri && <QrCode value={tfa.authenticatorUri} size={150} />}
                            </div>
                            <div className="mb-3 input-group">

                                <input name="code" type="number" className="form-control"
                                    ref={register({
                                        required: 'Code is required',
                                        minLength: {
                                            value: 6,
                                            message: "Code must have at least 6 characters"
                                        }
                                    })} />
                            </div>
                            <ErrorMessage error={errors.code} />
                            <button className="btn btn-success btn-block">Verify Code</button>
                        </form>
                    </div>
                </div>
            </div>
        </div>
    )
}
