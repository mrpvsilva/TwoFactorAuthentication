import React from 'react';
import { Container } from 'reactstrap';

export default function AuthCard({ children }) {
  return (
    <Container style={{ marginTop: '10%' }}>
      <div className="justify-content-center row">
        <div className="col-md-5 col-lg-4">
          <div className="card">
            <div className="p-4 card-body">{children}</div>
          </div>
        </div>
      </div>
    </Container>
  );
}
