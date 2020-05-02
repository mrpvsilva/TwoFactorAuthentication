import React, { useEffect, useState } from 'react';
import QRCode from 'qrcode.react';
import axios from 'axios';


export default function Login() {

    const [data, setData] = useState({ uri: '', hash: '' });
    const [code, setCode] = useState('');

    useEffect(() => {
        fetch('https://localhost:44308/api/Auth')
            .then(res => res.json())
            .then(json => setData(json));
    }, [])


    const handleChange = (e) => setCode(e.target.value);
    const handleSubmit = (e) => {
        e.preventDefault();

        axios
            .post('https://localhost:44308/api/Auth', { hash: data.hash, code })
            .then(res => console.log(res.data))
            .catch(error => console.log(error))
    }

    return (
        <>
            {data.uri && <QRCode style={{ marginBottom: '20px' }} value={data.uri} />}
            {
                data.hash &&
                <form onSubmit={handleSubmit}>
                    <input onChange={handleChange} style={{ marginBottom: '5px' }} />
                    <br />
                    <button>Enviar</button>
                </form>
            }
        </>
    )
}
