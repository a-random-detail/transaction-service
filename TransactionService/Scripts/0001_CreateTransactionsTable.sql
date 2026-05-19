CREATE TABLE transactions (
    id UUID PRIMARY KEY DEFAULT gen_random_uuid(),
    description VARCHAR(50) NOT NULL,
    transaction_date DATE NOT NULL,
    amount NUMERIC(19, 2) NOT NULL,
    CONSTRAINT chk_amount_positive CHECK (amount > 0)
);