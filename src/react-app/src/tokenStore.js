let _accessToken = null;
let _onTokenChange = null;

export const getAccessToken = () => _accessToken;

export const setAccessToken = (token) => {
  _accessToken = token;
  _onTokenChange?.(token);
};

export const registerTokenChangeCallback = (fn) => {
  _onTokenChange = fn;
};
