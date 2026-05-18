import React, { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { useNavigate } from 'react-router-dom';
import { Loader2 } from 'lucide-react';
import ErrorMessage from './ErrorMessage';
import AuthCard from './AuthCard';
import { passwordService } from '../services/passwordService';
import { Button } from 'src/components/ui/button';
import { Input } from 'src/components/ui/input';

export default function ResetPasswordCode() {
  const navigate = useNavigate();
  const { register, handleSubmit, formState: { errors, isSubmitting } } = useForm({
    defaultValues: { code: '' }
  });

  useEffect(() => {
    if (!sessionStorage.getItem('resetPassword')) navigate('/forgot-password');
  }, [navigate]);

  const onSubmit = async ({ code }) => {
    let email;
    try {
      ({ email } = JSON.parse(sessionStorage.getItem('resetPassword')));
    } catch {
      navigate('/forgot-password');
      return;
    }

    try {
      await passwordService.verifyCode(email, code);
      sessionStorage.setItem('resetPassword', JSON.stringify({ email, code }));
      navigate('/reset-password/new');
    } catch {}
  };

  return (
    <AuthCard>
      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <div>
          <h1 className="text-2xl font-bold">Enter Code</h1>
          <p className="text-sm text-muted-foreground">Enter the 6-digit code sent to your e-mail</p>
        </div>

        <div className="space-y-1">
          <Input
            type="text"
            inputMode="numeric"
            maxLength={6}
            placeholder="000000"
            className="text-base text-center tracking-[0.5em] font-bold text-lg"
            {...register('code', {
              required: 'Code is required',
              pattern: { value: /^[0-9]{6}$/, message: 'Code must be exactly 6 digits' }
            })}
          />
          <ErrorMessage error={errors.code} />
        </div>

        <Button type="submit" className="w-full" disabled={isSubmitting}>
          {isSubmitting ? <Loader2 className="h-4 w-4 animate-spin" /> : 'Verify Code'}
        </Button>
        <Button type="button" variant="outline" className="w-full" onClick={() => navigate('/forgot-password')}>
          Resend Code
        </Button>
      </form>
    </AuthCard>
  );
}
