import React, { useState } from 'react';
import { Button, Spinner } from 'reactstrap';
import { toast } from 'react-toastify';
import api from '../api';
import { useAuth } from '../AuthContext';

export default function ResetTwoFa() {
  const [loading, setLoading] = useState(false);
  const [confirmed, setConfirmed] = useState(false);
  const { logout } = useAuth();

  const handleReset = async () => {
    setLoading(true);
    try {
      await api.delete('/auth/totp');
      toast.success('Código TOTP resetado com sucesso. Faça login novamente para configurar um novo.');
      await logout();
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="row justify-content-center mt-4">
      <div className="col-md-6">
        <div className="card border-danger">
          <div className="card-body">
            <h5 className="card-title text-danger">Resetar autenticador TOTP</h5>
            <p className="card-text">
              Ao resetar seu autenticador, o código atual será removido permanentemente.
              Na próxima vez que você fizer login, precisará configurar um novo autenticador.
            </p>
            {!confirmed ? (
              <Button color="danger" outline onClick={() => setConfirmed(true)}>
                Resetar autenticador
              </Button>
            ) : (
              <div>
                <p className="text-danger fw-semibold">Tem certeza? Esta ação não pode ser desfeita.</p>
                <div className="d-flex gap-2">
                  <Button color="danger" onClick={handleReset} disabled={loading}>
                    {loading ? <Spinner size="sm" /> : 'Confirmar reset'}
                  </Button>
                  <Button color="secondary" outline onClick={() => setConfirmed(false)} disabled={loading}>
                    Cancelar
                  </Button>
                </div>
              </div>
            )}
          </div>
        </div>
      </div>
    </div>
  );
}
