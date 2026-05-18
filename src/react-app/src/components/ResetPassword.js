import React, { useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { useNavigate } from 'react-router-dom';
import { Lock, Loader2 } from 'lucide-react';
import { toast } from 'react-toastify';
import ErrorMessage from './ErrorMessage';
import AuthCard from './AuthCard';
import { passwordService } from '../services/passwordService';
import { PASSWORD_RULES } from '../validations';
import { Button } from 'src/components/ui/button';
import { Input } from 'src/components/ui/input';

export default function ResetPassword() {
  const navigate = useNavigate();
  const { register, handleSubmit, watch, formState: { errors, isSubmitting } } = useForm({
    defaultValues: { password: '', confirmPassword: '' }
  });

  const password = watch('password');

  useEffect(() => {
    try {
      const raw = sessionStorage.getItem('resetPassword');
      if (!raw) { navigate('/forgot-password'); return; }
      const parsed = JSON.parse(raw);
      if (!parsed.code) navigate('/reset-password/code');
    } catch {
      navigate('/forgot-password');
    }
  }, [navigate]);

  const onSubmit = async ({ password }) => {
    let email, code;
    try {
      ({ email, code } = JSON.parse(sessionStorage.getItem('resetPassword')));
    } catch {
      navigate('/forgot-password');
      return;
    }

    try {
      await passwordService.reset(email, code, password);
      sessionStorage.removeItem('resetPassword');
      toast.success('Password updated! Please log in');
      navigate('/login');
    } catch {}
  };

  return (
    <AuthCard>
      <form onSubmit={handleSubmit(onSubmit)} className="space-y-4">
        <div>
          <h1 className="text-2xl font-bold">New Password</h1>
          <p className="text-sm text-muted-foreground">Choose a strong password for your account</p>
        </div>

        <div className="space-y-1">
          <div className="relative">
            <Lock className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
            <Input
              placeholder="New password"
              autoComplete="new-password"
              type="password"
              className="pl-9 text-base"
              {...register('password', PASSWORD_RULES)}
            />
          </div>
          <ErrorMessage error={errors.password} />
        </div>

        <div className="space-y-1">
          <div className="relative">
            <Lock className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
            <Input
              placeholder="Confirm password"
              autoComplete="new-password"
              type="password"
              className="pl-9 text-base"
              {...register('confirmPassword', {
                validate: value => value === password || 'The passwords do not match'
              })}
            />
          </div>
          <ErrorMessage error={errors.confirmPassword} />
        </div>

        <Button type="submit" className="w-full" disabled={isSubmitting}>
          {isSubmitting ? <Loader2 className="h-4 w-4 animate-spin" /> : 'Save New Password'}
        </Button>
      </form>
    </AuthCard>
  );
}
