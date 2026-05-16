import React, { useState, useEffect } from 'react';
import { QRCodeSVG } from 'qrcode.react';
import { useForm } from 'react-hook-form';
import { useNavigate } from 'react-router-dom';
import { Container } from 'reactstrap';
import { toast } from 'react-toastify';
import { CopyToClipboard } from 'react-copy-to-clipboard';
import { Files } from 'react-bootstrap-icons';
import ErrorMessage from './ErrorMessage';
import Api from '../api';

export default function TwoFactAuth() {
  const navigate = useNavigate();
  const [tfa, setTfa] = useState({ authenticatorUri: '', hash: '', hasTwoFactorAuth: '', sharedKey: '' });
  const { register, handleSubmit, formState: { errors } } = useForm({
    defaultValues: { code: '' }
  });

  useEffect(() => {
    const tfaData = sessionStorage.getItem('tfa');
    if (!tfaData) {
      navigate('/login');
      return;
    }
    setTfa(JSON.parse(tfaData));
  }, [navigate]);

  const onSubmit = data => {
    const { hasTwoFactorAuth } = tfa;
    Api.post(`/auth/${hasTwoFactorAuth ? 'VerifyCode' : 'AddTwoFactAuth'}`, { ...data, ...tfa })
      .then(({ data }) => {
        if (data) {
          sessionStorage.removeItem('tfa');
          localStorage.token = JSON.stringify(data);
          navigate('/');
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
                <h3>Two Factor Authentication</h3>
                {tfa.sharedKey && (
                  <div>
                    <p>
                      Digitalize o QR code ou digite esta chave <kbd>{tfa.sharedKey}</kbd>{' '}
                      <CopyToClipboard
                        title="Copiar"
                        style={{ cursor: 'pointer' }}
                        text={tfa.sharedKey}
                        onCopy={() => toast.info('Chave copiada para área de transferência')}
                      >
                        <Files />
                      </CopyToClipboard>
                      {' '}no Google Authenticator.
                    </p>
                    <ul>
                      <li><a href="https://play.google.com/store/apps/details?id=com.google.android.apps.authenticator2">Android</a></li>
                      <li><a href="https://apps.apple.com/br/app/google-authenticator/id388497605">IPhone</a></li>
                    </ul>
                  </div>
                )}
                <div className="mb-3 justify-content-center row">
                  {tfa.authenticatorUri && <QRCodeSVG value={tfa.authenticatorUri} size={150} />}
                </div>
                <div className="mb-3 input-group">
                  <input
                    type="number"
                    className="form-control"
                    {...register('code', {
                      required: 'Code is required',
                      minLength: { value: 6, message: 'Code must have at least 6 characters' }
                    })}
                  />
                </div>
                <ErrorMessage error={errors.code} />
                <button className="btn btn-success w-100">Verify Code</button>
                <button type="button" className="btn btn-outline-secondary w-100 mt-2" onClick={() => navigate(-1)}>Back</button>
              </form>
            </div>
          </div>
        </div>
      </div>
    </Container>
  );
}
