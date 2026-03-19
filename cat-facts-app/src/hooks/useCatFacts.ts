import { useState } from 'react';
import { apiClient } from '@/api/client';
import type { CatFactsResponse } from '@/types/cat.types';
import { AxiosError } from 'axios';

export const useCatFacts = () => {
  const [fact, setFact] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [isLoading, setIsLoading] = useState<boolean>(false);

  const fetchFact = async () => {
    setIsLoading(true);
    setError(null);
    setFact(null);

    try {
      const response = await apiClient.get<CatFactsResponse>('/facts');
      
      const firstFact = response.data.data[0]?.fact;
      setFact(firstFact || 'Факты не найдены');
      
    } catch (err) {
      if (err instanceof AxiosError) {
        setError(`Код ${err.response?.status}: ${err.message}`);
      } else {
        setError('Произошла неизвестная ошибка');
      }
    } finally {
      setIsLoading(false);
    }
  };

  return { fact, error, isLoading, fetchFact };
};