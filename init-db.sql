-- Create the production database
CREATE DATABASE digitalwallet;

-- Grant permissions (optional, since we're using the default postgres user)
GRANT ALL PRIVILEGES ON DATABASE digitalwallet TO postgres;
GRANT ALL PRIVILEGES ON DATABASE digitalwallet_dev TO postgres;
