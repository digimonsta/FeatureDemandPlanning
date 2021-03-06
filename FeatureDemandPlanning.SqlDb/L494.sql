﻿-- Update the brochure model codes for L494 as they are currently missing

-- V8 Petrol 4x4 SWB
UPDATE OXO_Programme_Model SET BMC = 'SCBV' 
WHERE Id IN (1432, 1433, 1434, 1435)
AND
BMC IS NULL

-- V6 Petrol 4x4 SWB
UPDATE OXO_Programme_Model SET BMC = 'SDBV' 
WHERE Id IN (1436, 1461, 1462, 1463, 1464, 1466)
AND
BMC IS NULL

-- 2.0 Si4 Petrol 4x4 SWB
UPDATE OXO_Programme_Model SET BMC = 'SUBV' 
WHERE Id IN (1470, 1471, 1472, 1473, 1474)
AND
BMC IS NULL

-- SDV8 Diesel 4x4 SWB
UPDATE OXO_Programme_Model SET BMC = 'TRBV' 
WHERE Id IN (1475, 1476, 1477)
AND
BMC IS NULL

-- SDV6 Diesel 4x4 SWB
UPDATE OXO_Programme_Model SET BMC = 'TTBV' 
WHERE Id IN (1478, 1479, 1480, 1481, 1482)
AND
BMC IS NULL

-- TDV6 Diesel 4x4 SWB
UPDATE OXO_Programme_Model SET BMC = 'TWBV' 
WHERE Id IN (1483, 1484, 1485, 1486, 1487, 1488, 1489, 1490, 1491, 1492, 1493, 1494, 1495, 1496, 1497)
AND
BMC IS NULL