USE gassydb;
INSERT INTO RefreshToken(
  Token, 
  UserId, 
  Expires, 
  Created, 
  CreatedByIp, 
  Revoked, 
  RevokedByIp, 
  ReplacedByToken, 
  ReasonRevoked)
VALUES
  (
  "secret1",
  2, 
  DATE_ADD(CURRENT_TIME(), INTERVAL 12 MINUTE),
  DATE_ADD(CURRENT_TIME(), INTERVAL -3 MINUTE),
  "localhost",
  null,
  null,
  "secret2",
  null
 ),
 (
  "secret2",
  2, 
  DATE_ADD(CURRENT_TIME(), INTERVAL 12 MINUTE),
  DATE_ADD(CURRENT_TIME(), INTERVAL -3 MINUTE),
  "localhost",
  null,
  null,
  null,
  null
)



  
 