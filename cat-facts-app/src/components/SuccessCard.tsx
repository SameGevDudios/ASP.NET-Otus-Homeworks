interface SuccessCardProps {
  text: string;
}

export const SuccessCard = ({ text }: SuccessCardProps) => {
  return (
    <div style={{
      backgroundColor: '#f2fff5',
      color: '#117428',
      padding: '2rem',
      borderRadius: '12px',
      border: '2px solid #acd4b5',
      marginTop: '2rem'
    }}>
      <strong>Успех:</strong> {text}
    </div>
  );
};