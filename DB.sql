create database campuslove;
use campuslove;

CREATE TABLE genero (
id INT AUTO_INCREMENT PRIMARY KEY,
    descripcion VARCHAR(100)
);

CREATE TABLE orientacion (
    id INT AUTO_INCREMENT PRIMARY KEY,
    descripcion VARCHAR(100)
);

CREATE TABLE intereses (
    id INT AUTO_INCREMENT PRIMARY KEY,
    descripcion VARCHAR(100)
);

CREATE TABLE perfil (
    id INT AUTO_INCREMENT PRIMARY KEY,
    nombre VARCHAR(100),
    edad INT,
    generoId INT,
    orientacionId INT,
    descripcion TEXT,
    interesesId INT,
    FOREIGN KEY (generoId) REFERENCES genero(id),
    FOREIGN KEY (orientacionId) REFERENCES orientacion(id)
);

CREATE TABLE perfilInterese (
    id INT AUTO_INCREMENT PRIMARY KEY,
    interesesId INT,
    perfilId INT,
    FOREIGN KEY (interesesId) REFERENCES intereses(id),
    FOREIGN KEY (perfilId) REFERENCES perfil(id)
);

CREATE TABLE usuario (
    id INT AUTO_INCREMENT PRIMARY KEY,
    username VARCHAR(50),
    password VARCHAR(100),
    perfilId INT,
    likes INT,
    FOREIGN KEY (perfilId) REFERENCES perfil(id)
);

CREATE TABLE `match` (
    id INT AUTO_INCREMENT PRIMARY KEY,
    usuario1Id INT,
    usuario2Id INT,
    fecha DATE,
    FOREIGN KEY (usuario1Id) REFERENCES usuario(id),
    FOREIGN KEY (usuario2Id) REFERENCES usuario(id)
);

CREATE TABLE tipoInteraccion (
    id INT AUTO_INCREMENT PRIMARY KEY,
    descripcion VARCHAR(100)
);

CREATE TABLE interaccion (
    id INT AUTO_INCREMENT PRIMARY KEY,
    usuarioEnviaId INT,
    usuarioRecibeId INT,
    tipoInteraccionId INT,
    fecha DATE,
    FOREIGN KEY (usuarioEnviaId) REFERENCES usuario(id),
    FOREIGN KEY (usuarioRecibeId) REFERENCES usuario(id),
    FOREIGN KEY (tipoInteraccionId) REFERENCES tipoInteraccion(id)
);

CREATE TABLE estadisticas (
    id INT AUTO_INCREMENT PRIMARY KEY,
    usuarioId INT,
    likesDados INT,
    likesRecibidos INT,
    dislikesDados INT,
    dislikesRecibidos INT,
    match_hechos INT,
    FOREIGN KEY (usuarioId) REFERENCES usuario(id)
);



INSERT INTO genero (descripcion) VALUES
('Masculino'),
('Femenino'),
('No binario'),
('Prefiero no decirlo');

INSERT INTO orientacion (descripcion) VALUES
('Heterosexual'),
('Homosexual'),
('Bisexual'),
('Asexual'),
('Pansexual'),
('otro');


INSERT INTO intereses (descripcion) VALUES
('Leer'),
('Música'),
('Deportes'),
('Videojuegos'),
('Cocinar'),
('Arte'),
('naturaleza');


INSERT INTO perfil (nombre, edad, generoId, orientacionId, descripcion) VALUES
('Carlos Pérez', 23, 1, 1, 'Amante de la lectura y la tecnología.'),
('Ana García', 21, 2, 2, 'Me encanta viajar y conocer gente.'),
('Sam Jordan', 25, 3, 5, 'Explorando nuevas conexiones.');


INSERT INTO usuario (perfilid, Username, password) VALUES
(1, 'carlos23', 'hashedpassword1'),
(2, 'anita_g', 'hashedpassword2'),
(3, 'samj25', 'hashedpassword3');


INSERT INTO perfilinterese (perfilId, interesesId) VALUES
(1, 1),
(1, 2),
(2, 2),
(2, 4),
(3, 3),
(3, 5);




INSERT INTO `match` (usuario1Id, usuario2Id) VALUES
(1, 2),
(2, 3);


INSERT INTO TipoInteraccion (descripcion) VALUES ('Like'),('dislike');

