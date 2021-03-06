import React, { useState, useEffect } from 'react'
import QrCode from 'qrcode.react';
import { useForm } from 'react-hook-form';
import ErrorMessage from './ErrorMessage';
import Api from '../api';
import { Container } from 'reactstrap';
import { useAlert } from "react-alert";
import { CopyToClipboard } from 'react-copy-to-clipboard';
import { Files } from 'react-bootstrap-icons';

export default function TwoFactAuth({ history }) {

    const alert = useAlert();

    const [tfa, setTfa] = useState({
        authenticatorUri: '',
        hash: '',
        hasTwoFactorAuth: '',
        sharedKey: ''
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
                    localStorage.token = JSON.stringify(data);
                    history.push('/');
                    alert.success('Success')
                }
            })
            .catch(err => alert.error('Request error'))
    }

    const handleClick = () => {
        alert.info("Chave copiada para área de transferência");
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
        <Container style={{ marginTop: '10%' }}>
            <div className="justify-content-center row">
                <div className="col-md-9 col-lg-7 col-xl-6">
                    <div className="mx-4 card">
                        <div className="p-4 card-body">
                            <form onSubmit={handleSubmit(onSubmit)}>
                                <h3>Two Factor Authentication</h3>
                                {
                                    tfa.sharedKey && (
                                        <div className="">
                                            <p>Digitalize o QR code ou digite esta chave <kbd>{tfa.sharedKey}</kbd><CopyToClipboard title="Copiar" style={{ cursor: "pointer" }} text={tfa.sharedKey} onCopy={handleClick}><Files /></CopyToClipboard> no Google Authenticator.</p>
                                            <ul>
                                                <li>
                                                    <a href="https://play.google.com/store/apps/details?id=com.google.android.apps.authenticator2">Android</a>
                                                </li>
                                                <li>
                                                    <a href="https://apps.apple.com/br/app/google-authenticator/id388497605">IPhone</a>
                                                </li>
                                            </ul>


                                        </div>
                                    )
                                }
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
        </Container >
    )
}
