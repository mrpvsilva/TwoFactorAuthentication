import React from 'react'

export default function ErrorMessage({ error }) {
    return (error ? (<p style={{ color: '#dc3545', fontSize: '.9em' }}>{error.message}</p>) : (<></>))
}
