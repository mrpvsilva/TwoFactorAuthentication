import React from 'react';
import { useForm } from 'react-hook-form';
import { Lock, Loader2 } from 'lucide-react';
import { useNavigate } from 'react-router-dom';
import { toast } from 'react-toastify';
import ErrorMessage from './ErrorMessage';
import AuthCard from './AuthCard';
import { authService } from '../services/authService';
import { EMAIL_PATTERN } from '../validations';
import { Button } from 'src/components/ui/button';
import { Input } from 'src/components/ui/input';

export default function Register() {
  const navigate = useNavigate();
  const { register, handleSubmit, watch, formState: { errors, isSubmitting } } = useForm({
    defaultValues: { email: '', password: '', repeatpassword: '' }
  });

  const password = watch('password', '');

  const onSubmit = async (data) => {
    try {
      const { data: result } = await authService.register(data);
      sessionStorage.setItem('registration', JSON.stringify({ hash: result.hash }));
      toast.success('Cadastro realizado! Verifique o código enviado ao seu e-mail.');
      navigate('/verify-registration');
    } catch {}
  };

  return (
    <AuthCard>
      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <div>
          <h1 className="text-2xl font-bold">Register</h1>
          <p className="text-sm text-muted-foreground">Create your account</p>
        </div>

        <div className="space-y-1">
          <div className="relative">
            <span className="absolute left-3 top-1/2 -translate-y-1/2 text-sm font-medium text-muted-foreground">@</span>
            <Input
              placeholder="Email"
              autoComplete="email"
              type="text"
              className="pl-8 text-base"
              {...register('email', { required: 'E-mail is required', pattern: EMAIL_PATTERN })}
            />
          </div>
          <ErrorMessage error={errors.email} />
        </div>

        <div className="space-y-1">
          <div className="relative">
            <Lock className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
            <Input
              placeholder="Password"
              autoComplete="new-password"
              type="password"
              className="pl-9 text-base"
              {...register('password', {
                required: 'Password is required',
                minLength: { value: 8, message: 'Password must have at least 8 characters' }
              })}
            />
          </div>
          <ErrorMessage error={errors.password} />
        </div>

        <div className="space-y-1">
          <div className="relative">
            <Lock className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
            <Input
              placeholder="Repeat password"
              autoComplete="new-password"
              type="password"
              className="pl-9 text-base"
              {...register('repeatpassword', {
                validate: value => value === password || 'The passwords do not match'
              })}
            />
          </div>
          <ErrorMessage error={errors.repeatpassword} />
        </div>

        <Button type="submit" className="w-full" disabled={isSubmitting}>
          {isSubmitting ? <Loader2 className="h-4 w-4 animate-spin" /> : 'Create Account'}
        </Button>
        <Button type="button" variant="outline" className="w-full" onClick={() => navigate(-1)} disabled={isSubmitting}>
          Back
        </Button>
      </form>
    </AuthCard>
  );
}
