CREATE TABLE Filling (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) UNIQUE
);

CREATE TABLE Cakes (
    id SERIAL PRIMARY KEY,
    name VARCHAR(100) UNIQUE
);

CREATE TABLE Buyers (
    id SERIAL PRIMARY KEY,
    name VARCHAR(1000) NOT NULL,
  weight_cake FLOAT
);

CREATE TABLE Orders (
    id SERIAL PRIMARY KEY,
    Order_number VARCHAR(100) NOT NULL,
    Filling_id INT REFERENCES Filling(id),
    Cake_id INT REFERENCES Cakes(id),
    Buyer_id INT
  FOREIGN KEY (Buyer_id) REFERENCES Buyers(id)
);

INSERT INTO Filling (name) VALUES ('Крем'), ('Варенье'), ('Карамель');
INSERT INTO Cakes (name) VALUES ('Ванильный'), ('Шоколадный'), ('Маковый');
INSERT INTO Buyers (name, weight_cake) VALUES ('Мария, 2'), ('Илья, 5'), ('Наталья, 1');

INSERT INTO Orders (Order_number, Filling_id, Cake_id, Buyer_id) VALUES 
('ABCD123456', 1, 1, 1),
('EFGH789012', 2, 2, 2),
('IJKL345678', 3, 3, 3);

CREATE USER user WITH PASSWORD 'userpass';
GRANT ALL PRIVILEGES ON ALL TABLES IN SCHEMA public TO user;
GRANT ALL PRIVILEGES ON DATABASE sdl TO user;