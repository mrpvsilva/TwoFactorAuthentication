import React, { useState, useEffect } from 'react';
import { QRCodeSVG } from 'qrcode.react';
import { useForm } from 'react-hook-form';
import { useNavigate } from 'react-router-dom';
import { Spinner } from 'reactstrap';
import { toast } from 'react-toastify';
import { CopyToClipboard } from 'react-copy-to-clipboard';
import { Files } from 'react-bootstrap-icons';
import ErrorMessage from './ErrorMessage';
import AuthCard from './AuthCard';
import { useAuth } from '../AuthContext';
import { authService } from '../services/authService';

const TFA_INITIAL = { authenticatorUri: '', hash: '', hasTwoFactorAuth: false, sharedKey: '' };

export default function TwoFactAuth() {
  const navigate = useNavigate();
  const { setAccessToken } = useAuth();
  const [tfa, setTfa] = useState(TFA_INITIAL);
  const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm({
    defaultValues: { code: '' }
  });

  useEffect(() => {
    const raw = sessionStorage.getItem('tfa');
    if (!raw) { navigate('/login'); return; }
    try {
      setTfa(JSON.parse(raw));
    } catch {
      navigate('/login');
    }
  }, [navigate]);

  const onSubmit = async ({ code }) => {
    try {
      const endpoint = tfa.hasTwoFactorAuth ? authService.verifyCode : authService.addTwoFactor;
      const { data } = await endpoint({ code, ...tfa });
      if (data?.accessToken) {
        sessionStorage.removeItem('tfa');
        setAccessToken(data.accessToken);
        toast.success('Success');
        navigate('/');
      }
    } catch {}
  };

  return (
    <AuthCard>
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
              <li>
                <a href="https://play.google.com/store/apps/details?id=com.google.android.apps.authenticator2" target="_blank" rel="noopener noreferrer">
                  Android
                </a>
              </li>
              <li>
                <a href="https://apps.apple.com/br/app/google-authenticator/id388497605" target="_blank" rel="noopener noreferrer">
                  iPhone
                </a>
              </li>
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
        <button className="btn btn-success w-100" disabled={isSubmitting}>
          {isSubmitting ? <Spinner size="sm" /> : 'Verify Code'}
        </button>
        <button type="button" className="btn btn-outline-secondary w-100 mt-2" onClick={() => navigate(-1)} disabled={isSubmitting}>
          Back
        </button>
      </form>
    </AuthCard>
  );
}
