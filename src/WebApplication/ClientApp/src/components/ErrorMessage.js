import React from 'react'
import styled from 'styled-components';

const P = styled.p`
color: #dc3545;
font-size: .9em;
`;

export default function ErrorMessage({ error }) {
    return (error ? (<P>{error.message}</P>) : (<></>))
}
