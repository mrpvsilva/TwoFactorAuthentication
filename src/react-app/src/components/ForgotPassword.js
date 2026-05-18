import React from 'react';
import { useForm } from 'react-hook-form';
import { useNavigate } from 'react-router-dom';
import { Loader2 } from 'lucide-react';
import { toast } from 'react-toastify';
import ErrorMessage from './ErrorMessage';
import AuthCard from './AuthCard';
import { passwordService } from '../services/passwordService';
import { EMAIL_PATTERN } from '../validations';
import { Button } from 'src/components/ui/button';
import { Input } from 'src/components/ui/input';

export default function ForgotPassword() {
  const navigate = useNavigate();
  const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm({
    defaultValues: { email: '' }
  });

  const onSubmit = async ({ email }) => {
    try {
      await passwordService.forgot(email);
      sessionStorage.setItem('resetPassword', JSON.stringify({ email }));
      toast.success('Code sent! Check your inbox');
      navigate('/reset-password/code');
    } catch {}
  };

  return (
    <AuthCard>
      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <div>
          <h1 className="text-2xl font-bold">Forgot Password</h1>
          <p className="text-sm text-muted-foreground">Enter your e-mail to receive a reset code</p>
        </div>

        <div className="space-y-1">
          <Input
            placeholder="E-mail"
            autoComplete="email"
            type="text"
            className="text-base"
            {...register('email', { required: 'E-mail is required', pattern: EMAIL_PATTERN })}
          />
          <ErrorMessage error={errors.email} />
        </div>

        <Button type="submit" className="w-full" disabled={isSubmitting}>
          {isSubmitting ? <Loader2 className="h-4 w-4 animate-spin" /> : 'Send Code'}
        </Button>
        <Button type="button" variant="outline" className="w-full" onClick={() => navigate('/login')}>
          Back to Login
        </Button>
      </form>
    </AuthCard>
  );
}
