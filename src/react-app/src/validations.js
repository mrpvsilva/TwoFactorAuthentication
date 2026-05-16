export const EMAIL_PATTERN = {
  value: /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,4}$/i,
  message: 'E-mail is invalid',
};

export const PASSWORD_RULES = {
  required: 'Password is required',
  minLength: { value: 8, message: 'Password must be at least 8 characters' },
  pattern: {
    value: /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d])/,
    message: 'Password must contain uppercase, lowercase, number and special character',
  },
};
