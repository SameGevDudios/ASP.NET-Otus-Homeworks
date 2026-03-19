import { useCatFacts } from '@/hooks/useCatFacts';
import { SuccessCard } from '@/components/SuccessCard';
import { ErrorCard } from '@/components/ErrorCard';

export const CatFactViewer = () => {
  const { fact, error, isLoading, fetchFact } = useCatFacts();

  return (
    <div style={{ padding: '2rem', fontFamily: 'sans-serif' }}>
      <h2>Получить факт о котах</h2>
      
      <button 
        onClick={fetchFact} 
        disabled={isLoading}
        style={{ 
          padding: '10px 20px', 
          cursor: 'pointer', 
          fontSize: '16px', 
          borderRadius: 16,
          marginTop: '12px'
         }}
      >
        {isLoading ? 'Загрузка...' : 'Запросить данные'}
      </button>

      {fact && <SuccessCard text={fact} />}

      {error && <ErrorCard message={error} />}
    </div>
  );
};