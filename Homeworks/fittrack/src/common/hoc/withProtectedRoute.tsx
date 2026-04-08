import { useEffect, type ComponentType, type FC } from 'react';
import { useNavigate } from 'react-router-dom';
import { useAppSelector } from '@/app/store/hooks';
import { MainLayout } from '@/common/ui/MainLayout';
import { Box, CircularProgress } from '@mui/material';

export function withProtectedRoute<P extends object>(WrappedComponent: ComponentType<P>) {
  const Wrapped: FC<P> = (props) => {
    const { isAuth, isLoading, user } = useAppSelector((state) => state.auth);
    const navigate = useNavigate();

    useEffect(() => {
      if (!isLoading && !isAuth) {
        navigate('/login', { replace: true });
      }
    }, [isAuth, isLoading, navigate]);

    if (isLoading) {
      return (
        <Box sx={{ display: 'flex', justifyContent: 'center', mt: 10 }}>
          <CircularProgress />
        </Box>
      );
    }

    if (!isAuth) return null;

    return (
      <MainLayout user={user}>
        <WrappedComponent {...props} />
      </MainLayout>
    );
  };

  return Wrapped;
}