interface ErrorCardProps {
  message: string;
}

export const ErrorCard = ({ message }: ErrorCardProps) => {
  return (
    <div style={{
      backgroundColor: '#ffe7e9',
      color: '#53161d',
      padding: '2rem',
      borderRadius: '12px',
      border: '2px solid #e9b9be',
      marginTop: '2rem'
    }}>
      <strong>Ошибка запроса:</strong> {message}
    </div>
  );
};