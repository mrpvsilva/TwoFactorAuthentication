let _navigate;

export const setNavigate = (navigate) => { _navigate = navigate; };

export const navigate = (to, options) => _navigate?.(to, options);
