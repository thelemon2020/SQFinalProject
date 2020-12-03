-- MySqlBackup.NET 2.3.3
-- Dump Time: 2020-12-02 20:30:11
-- --------------------------------------
-- Server version 8.0.21 MySQL Community Server - GPL


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;


-- 
-- Definition of account
-- 

DROP TABLE IF EXISTS `account`;
CREATE TABLE IF NOT EXISTS `account` (
  `accountid` int NOT NULL,
  `clientName` varchar(45) DEFAULT NULL,
  `balance` double DEFAULT NULL,
  `lastPayment` date DEFAULT NULL,
  PRIMARY KEY (`accountid`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table account
-- 

/*!40000 ALTER TABLE `account` DISABLE KEYS */;

/*!40000 ALTER TABLE `account` ENABLE KEYS */;

-- 
-- Definition of carrier
-- 

DROP TABLE IF EXISTS `carrier`;
CREATE TABLE IF NOT EXISTS `carrier` (
  `carrierid` int NOT NULL AUTO_INCREMENT,
  `carrierName` varchar(45) NOT NULL,
  `FTLRate` double NOT NULL,
  `LTLRate` double NOT NULL,
  `reefCharge` double NOT NULL,
  PRIMARY KEY (`carrierid`),
  UNIQUE KEY `carrierName_UNIQUE` (`carrierName`)
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table carrier
-- 

/*!40000 ALTER TABLE `carrier` DISABLE KEYS */;
INSERT INTO `carrier`(`carrierid`,`carrierName`,`FTLRate`,`LTLRate`,`reefCharge`) VALUES
(1,'Planet Express',5.21,0.3621,0.08),
(2,'Schooner''s',5.05,0.3434,0.07),
(3,'Tillman Transport',5.11,0.3012,0.09),
(4,'We Haul',5.2,0,0.065);
/*!40000 ALTER TABLE `carrier` ENABLE KEYS */;

-- 
-- Definition of contract
-- 

DROP TABLE IF EXISTS `contract`;
CREATE TABLE IF NOT EXISTS `contract` (
  `contractId` int NOT NULL,
  `orderID` int NOT NULL,
  `carrierID` int NOT NULL,
  `estCost` double DEFAULT NULL,
  `estTime` double DEFAULT NULL,
  PRIMARY KEY (`contractId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table contract
-- 

/*!40000 ALTER TABLE `contract` DISABLE KEYS */;

/*!40000 ALTER TABLE `contract` ENABLE KEYS */;

-- 
-- Definition of depot
-- 

DROP TABLE IF EXISTS `depot`;
CREATE TABLE IF NOT EXISTS `depot` (
  `carrierID` int NOT NULL,
  `depotCity` varchar(45) NOT NULL,
  `FTLA` int DEFAULT NULL,
  `LTLA` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`carrierID`,`depotCity`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci COMMENT='	';

-- 
-- Dumping data for table depot
-- 

/*!40000 ALTER TABLE `depot` DISABLE KEYS */;
INSERT INTO `depot`(`carrierID`,`depotCity`,`FTLA`,`LTLA`) VALUES
(1,'Belleville',50,'640'),
(1,'Hamilton',50,'640'),
(1,'Oshawa',50,'640'),
(1,'Ottawa',50,'640'),
(1,'Windsor',50,'640'),
(2,'Kingston',18,'98'),
(2,'London',18,'98'),
(2,'Toronto',18,'98'),
(3,'Hamilton',18,'45'),
(3,'London',18,'45'),
(3,'Windsor',24,'35'),
(4,'Ottawa',11,'0'),
(4,'Toronto',11,'0');
/*!40000 ALTER TABLE `depot` ENABLE KEYS */;

-- 
-- Definition of invoice
-- 

DROP TABLE IF EXISTS `invoice`;
CREATE TABLE IF NOT EXISTS `invoice` (
  `invoiceID` int NOT NULL,
  `contractID` int NOT NULL,
  `accountID` int DEFAULT NULL,
  `invoiceDate` date DEFAULT NULL,
  `cost` double DEFAULT NULL,
  PRIMARY KEY (`invoiceID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table invoice
-- 

/*!40000 ALTER TABLE `invoice` DISABLE KEYS */;

/*!40000 ALTER TABLE `invoice` ENABLE KEYS */;

-- 
-- Definition of login
-- 

DROP TABLE IF EXISTS `login`;
CREATE TABLE IF NOT EXISTS `login` (
  `id` int NOT NULL AUTO_INCREMENT,
  `username` varchar(45) DEFAULT NULL,
  `password` varchar(45) DEFAULT NULL,
  `role` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table login
-- 

/*!40000 ALTER TABLE `login` DISABLE KEYS */;
INSERT INTO `login`(`id`,`username`,`password`,`role`) VALUES
(1,'admin','admin','a');
/*!40000 ALTER TABLE `login` ENABLE KEYS */;

-- 
-- Definition of rates
-- 

DROP TABLE IF EXISTS `rates`;
CREATE TABLE IF NOT EXISTS `rates` (
  `ftlrate` int NOT NULL,
  `ltl` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`ftlrate`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table rates
-- 

/*!40000 ALTER TABLE `rates` DISABLE KEYS */;
INSERT INTO `rates`(`ftlrate`,`ltl`) VALUES
(8,'5');
/*!40000 ALTER TABLE `rates` ENABLE KEYS */;

-- 
-- Definition of route
-- 

DROP TABLE IF EXISTS `route`;
CREATE TABLE IF NOT EXISTS `route` (
  `routeID` int NOT NULL,
  `destCity` varchar(45) NOT NULL,
  `kmToEast` int DEFAULT NULL,
  `kmtoWest` int DEFAULT NULL,
  `hToEast` double DEFAULT NULL,
  `htoWest` double DEFAULT NULL,
  `east` varchar(45) DEFAULT NULL,
  `west` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`destCity`,`routeID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table route
-- 

/*!40000 ALTER TABLE `route` DISABLE KEYS */;
INSERT INTO `route`(`routeID`,`destCity`,`kmToEast`,`kmtoWest`,`hToEast`,`htoWest`,`east`,`west`) VALUES
(6,'Belleville',82,134,1.2,1.65,'Kingston','Oshawa'),
(3,'Hamilton',68,128,1.25,1.75,'Toronto','London'),
(7,'Kingston',196,82,2.5,1.2,'Ottawa','Belleville'),
(2,'London',128,191,1.75,2.5,'Hamilton','Windsor'),
(5,'Oshawa',134,60,1.65,1.3,'Belleville','Toronto'),
(8,'Ottawa',0,196,0,2.5,'END','Kingston'),
(4,'Toronto',60,68,1.3,1.25,'Oshawa','Hamilton'),
(1,'Windsor',191,0,2.5,0,'London','END');
/*!40000 ALTER TABLE `route` ENABLE KEYS */;

-- 
-- Definition of order
-- 

DROP TABLE IF EXISTS `order`;
CREATE TABLE IF NOT EXISTS `order` (
  `orderID` int NOT NULL,
  `clientname` varchar(45) DEFAULT NULL,
  `jobtype` int DEFAULT NULL,
  `skidQuant` varchar(45) DEFAULT NULL,
  `depotCity` varchar(45) NOT NULL,
  `destCity` varchar(45) DEFAULT NULL,
  `vanType` varchar(45) DEFAULT NULL,
  PRIMARY KEY (`orderID`),
  KEY `destCity_idx` (`destCity`),
  KEY `depotCity_idx` (`depotCity`),
  CONSTRAINT `destCity` FOREIGN KEY (`destCity`) REFERENCES `route` (`destCity`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table order
-- 

/*!40000 ALTER TABLE `order` DISABLE KEYS */;

/*!40000 ALTER TABLE `order` ENABLE KEYS */;

-- 
-- Definition of tripline
-- 

DROP TABLE IF EXISTS `tripline`;
CREATE TABLE IF NOT EXISTS `tripline` (
  `contractID` int NOT NULL,
  `tripID` int NOT NULL,
  `tripCost` double DEFAULT NULL,
  `tripTime` double DEFAULT NULL,
  `quantity` double DEFAULT NULL,
  PRIMARY KEY (`contractID`,`tripID`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

-- 
-- Dumping data for table tripline
-- 

/*!40000 ALTER TABLE `tripline` DISABLE KEYS */;

/*!40000 ALTER TABLE `tripline` ENABLE KEYS */;


/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;


-- Dump completed on 2020-12-02 20:30:11
-- Total time: 0:0:0:0:154 (d:h:m:s:ms)
