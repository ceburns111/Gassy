USE gassydb;
CREATE TABLE gassydb.RefreshToken (
  Id INT UNSIGNED AUTO_INCREMENT PRIMARY KEY,
  Token VARCHAR(200) NOT NULL,
  UserId INT NOT NULL,
  Expires DATETIME NOT NULL,
  Created DATETIME NOT NULL,
  CreatedByIp VARCHAR(100) NOT NULL,
  Revoked DATETIME,
  RevokedByIp VARCHAR(100),
  ReplacedByToken VARCHAR(200),
  ReasonRevoked VARCHAR(100),
  UNIQUE(Id),
  UNIQUE(Token),
  UNIQUE(ReplacedByToken),
  INDEX(Id),
  INDEX(UserId)
);

