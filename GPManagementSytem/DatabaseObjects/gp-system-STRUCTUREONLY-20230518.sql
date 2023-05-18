CREATE DATABASE  IF NOT EXISTS `gp-system` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;
USE `gp-system`;
-- MySQL dump 10.13  Distrib 8.0.29, for Win64 (x86_64)
--
-- Host: 127.0.0.1    Database: gp-system
-- ------------------------------------------------------
-- Server version	8.0.29

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!50503 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Table structure for table `allocation`
--

DROP TABLE IF EXISTS `allocation`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `allocation` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `PracticeId` int DEFAULT NULL,
  `Year2Wk1Requested` varchar(5) DEFAULT NULL,
  `Year2Wk1Allocated` varchar(5) DEFAULT NULL,
  `Year2Wk2Requested` varchar(5) DEFAULT NULL,
  `Year2Wk2Allocated` varchar(5) DEFAULT NULL,
  `Year2Wk3Requested` varchar(5) DEFAULT NULL,
  `Year2Wk3Allocated` varchar(5) DEFAULT NULL,
  `Year2Wk4Requested` varchar(5) DEFAULT NULL,
  `Year2Wk4Allocated` varchar(5) DEFAULT NULL,
  `Year2Wk5Requested` varchar(5) DEFAULT NULL,
  `Year2Wk5Allocated` varchar(5) DEFAULT NULL,
  `Year2Wk6Requested` varchar(5) DEFAULT NULL,
  `Year2Wk6Allocated` varchar(5) DEFAULT NULL,
  `Year3B1Requested` varchar(5) DEFAULT NULL,
  `Year3B1Allocated` varchar(5) DEFAULT NULL,
  `Year3B2Requested` varchar(5) DEFAULT NULL,
  `Year3B2Allocated` varchar(5) DEFAULT NULL,
  `Year3B3Requested` varchar(5) DEFAULT NULL,
  `Year3B3Allocated` varchar(5) DEFAULT NULL,
  `Year3B4Requested` varchar(5) DEFAULT NULL,
  `Year3B4Allocated` varchar(5) DEFAULT NULL,
  `Year3B5Requested` varchar(5) DEFAULT NULL,
  `Year3B5Allocated` varchar(5) DEFAULT NULL,
  `Year3B6Requested` varchar(5) DEFAULT NULL,
  `Year3B6Allocated` varchar(5) DEFAULT NULL,
  `Year3B7Requested` varchar(5) DEFAULT NULL,
  `Year3B7Allocated` varchar(5) DEFAULT NULL,
  `Year4B1Requested` varchar(5) DEFAULT NULL,
  `Year4B1Allocated` varchar(5) DEFAULT NULL,
  `Year4B2Requested` varchar(5) DEFAULT NULL,
  `Year4B2Allocated` varchar(5) DEFAULT NULL,
  `Year4B3Requested` varchar(5) DEFAULT NULL,
  `Year4B3Allocated` varchar(5) DEFAULT NULL,
  `Year4B4Requested` varchar(5) DEFAULT NULL,
  `Year4B4Allocated` varchar(5) DEFAULT NULL,
  `Year4B5Requested` varchar(5) DEFAULT NULL,
  `Year4B5Allocated` varchar(5) DEFAULT NULL,
  `Year4B6Requested` varchar(5) DEFAULT NULL,
  `Year4B6Allocated` varchar(5) DEFAULT NULL,
  `Year4B7Requested` varchar(5) DEFAULT NULL,
  `Year4B7Allocated` varchar(5) DEFAULT NULL,
  `Year4B8Requested` varchar(5) DEFAULT NULL,
  `Year4B8Allocated` varchar(5) DEFAULT NULL,
  `Year5B1Requested` varchar(5) DEFAULT NULL,
  `Year5B1Allocated` varchar(5) DEFAULT NULL,
  `Year5B2Requested` varchar(5) DEFAULT NULL,
  `Year5B2Allocated` varchar(5) DEFAULT NULL,
  `Year5B3Requested` varchar(5) DEFAULT NULL,
  `Year5B3Allocated` varchar(5) DEFAULT NULL,
  `Year5B4Requested` varchar(5) DEFAULT NULL,
  `Year5B4Allocated` varchar(5) DEFAULT NULL,
  `Year5B5Requested` varchar(5) DEFAULT NULL,
  `Year5B5Allocated` varchar(5) DEFAULT NULL,
  `Year5B6Requested` varchar(5) DEFAULT NULL,
  `Year5B6Allocated` varchar(5) DEFAULT NULL,
  `AcademicYear` varchar(45) DEFAULT NULL,
  `ServiceContractReceived` int DEFAULT NULL,
  `ConfirmationName` varchar(250) DEFAULT NULL,
  `AllocationApproved` bit(1) DEFAULT NULL,
  `DateCreated` datetime DEFAULT NULL,
  `DateUpdated` datetime DEFAULT NULL,
  `UpdatedBy` int DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `emailtemplates`
--

DROP TABLE IF EXISTS `emailtemplates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `emailtemplates` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `EmailTypeId` int NOT NULL,
  `Subject` varchar(500) NOT NULL,
  `Body` text NOT NULL,
  `AttachmentName` varchar(500) DEFAULT NULL,
  `AttachmentName2` varchar(500) DEFAULT NULL,
  `DateUpdated` datetime DEFAULT NULL,
  `UpdatedBy` int NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `orignaldataimport`
--

DROP TABLE IF EXISTS `orignaldataimport`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `orignaldataimport` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `OriginalID` int DEFAULT NULL,
  `Surgery` text,
  `Surgery not in use` int DEFAULT NULL,
  `GP 2` text,
  `GP 2 Email` text,
  `Website Address` text,
  `GP 3` text,
  `GP 3 Email` text,
  `GP 4` text,
  `GP 4 Email` text,
  `GP 1` text,
  `Address Line 1` text,
  `Address Line 2` text,
  `Town/City` text,
  `Postcode` text,
  `Phone Number` text,
  `Fax Number` text,
  `GP 1 Email` text,
  `PM Email` text,
  `Supplier Number` int DEFAULT NULL,
  `Practice Manager` text,
  `1st (Requested)` int DEFAULT NULL,
  `1st (Allocated)` int DEFAULT NULL,
  `2nd (Requested)` int DEFAULT NULL,
  `2nd (Allocated)` int DEFAULT NULL,
  `3rd (Requested)` int DEFAULT NULL,
  `3rd (Allocated)` int DEFAULT NULL,
  `2nd - Block 1 (R)` int DEFAULT NULL,
  `2nd - Block 1 (A)` int DEFAULT NULL,
  `2nd - Block 2 (R)` int DEFAULT NULL,
  `2nd - Block 2 (A)` int DEFAULT NULL,
  `2nd - Block 3 (R)` int DEFAULT NULL,
  `2nd - Block 3 (A)` int DEFAULT NULL,
  `2nd - Block 4 (R)` int DEFAULT NULL,
  `2nd - Block 4 (A)` int DEFAULT NULL,
  `3rd - Block 1 (R)` int DEFAULT NULL,
  `3rd - Block 1 (A)` int DEFAULT NULL,
  `3rd - Block 2 (R)` int DEFAULT NULL,
  `3rd - Block 3 (R)` int DEFAULT NULL,
  `3rd - Block 4 (R)` int DEFAULT NULL,
  `3rd - Block 5 (R)` int DEFAULT NULL,
  `4th (Requested)` int DEFAULT NULL,
  `4th (Allocated)` int DEFAULT NULL,
  `5th (Requested)` int DEFAULT NULL,
  `5th (Allocated)` int DEFAULT NULL,
  `Grad 1 (Requested)` int DEFAULT NULL,
  `Grad 1 (Allocated)` int DEFAULT NULL,
  `Grad 2 (Requested)` int DEFAULT NULL,
  `Grad 2 (Allocated)` int DEFAULT NULL,
  `1st Day (R)` text,
  `2nd Day (R)` text,
  `3rd Day (R)` text,
  `5th - Block 1 (R)` int DEFAULT NULL,
  `5th - Block 2 (R)` int DEFAULT NULL,
  `5th - Block 3 (R)` int DEFAULT NULL,
  `5th - Block 4 (R)` int DEFAULT NULL,
  `5th - Block 5 (R)` int DEFAULT NULL,
  `Grad 1 Day (R)` text,
  `Grad 2 Day (R)` text,
  `Do Not Contact Surgery` int DEFAULT NULL,
  `Attachments Allocated` text,
  `Service Contract Received` text,
  `Notes` text,
  `Contract Received` int DEFAULT NULL,
  `UCCT Notes` text,
  `Quality Visit date R1` text,
  `Quality Visit notes` text,
  `Active` int DEFAULT NULL,
  `Disabled` int DEFAULT NULL,
  `Queried` int DEFAULT NULL,
  `List Size` text,
  `1 Student` int DEFAULT NULL,
  `2 Students` int DEFAULT NULL,
  `Monday` int DEFAULT NULL,
  `Tuesday` int DEFAULT NULL,
  `Wednesday` int DEFAULT NULL,
  `Thursday` int DEFAULT NULL,
  `Friday` int DEFAULT NULL,
  `R 3rd Block 1` int DEFAULT NULL,
  `R 3rd Block 2` int DEFAULT NULL,
  `R 3rd Block 3` int DEFAULT NULL,
  `R 3rd Block 4` int DEFAULT NULL,
  `R 3rd Block 5` int DEFAULT NULL,
  `R 3rd Block 6` int DEFAULT NULL,
  `R 4th Block 1` int DEFAULT NULL,
  `R 4th Block 2` int DEFAULT NULL,
  `R 4th Block 3` int DEFAULT NULL,
  `R 4th Block 4` int DEFAULT NULL,
  `R 4th Block 5` int DEFAULT NULL,
  `R 4th Block 6` int DEFAULT NULL,
  `R 4th Block 7` int DEFAULT NULL,
  `R 4th Block 8` int DEFAULT NULL,
  `R 4th Block 9` int DEFAULT NULL,
  `R 5th Block 1` int DEFAULT NULL,
  `R 5th Block 2` int DEFAULT NULL,
  `R 5th Block 3` int DEFAULT NULL,
  `R 5th Block 4` int DEFAULT NULL,
  `A 3rd Block 1` int DEFAULT NULL,
  `A 3rd Block 2` int DEFAULT NULL,
  `A 3rd Block 3` int DEFAULT NULL,
  `A 3rd Block 4` int DEFAULT NULL,
  `A 3rd Block 5` int DEFAULT NULL,
  `A 3rd Block 6` int DEFAULT NULL,
  `A 4th Block 1` int DEFAULT NULL,
  `A 4th Block 2` int DEFAULT NULL,
  `A 4th Block 3` int DEFAULT NULL,
  `A 4th Block 4` int DEFAULT NULL,
  `A 4th Block 5` int DEFAULT NULL,
  `A 4th Block 6` int DEFAULT NULL,
  `A 4th Block 7` int DEFAULT NULL,
  `A 4th Block 8` int DEFAULT NULL,
  `A 4th Block 9` int DEFAULT NULL,
  `A 5th Block 1` int DEFAULT NULL,
  `A 5th Block 2` int DEFAULT NULL,
  `A 5th Block 3` int DEFAULT NULL,
  `A 5th Block 4` int DEFAULT NULL,
  `New Practice` int DEFAULT NULL,
  `Academic Year` text,
  `Contract Received1` int DEFAULT NULL,
  `New Supplier Form` int DEFAULT NULL,
  `End Year Evaluation` int DEFAULT NULL,
  `HC Active` int DEFAULT NULL,
  `HC Disabled` int DEFAULT NULL,
  `HC Queried` int DEFAULT NULL,
  `HC GP 1 Name` text,
  `HC Expiry 1` text,
  `HC GP 2 Name` text,
  `HC Expiry 2` text,
  `Quality Visit Date` text,
  `OK to Proceed` int DEFAULT NULL,
  `Data Review Date` text,
  `Tutor Training GP Name` text,
  `Tutor Training Date` text,
  `R 3rd Block 7` int DEFAULT NULL,
  `A 3rd Blcok 7` int DEFAULT NULL,
  `A 2nd Block 5` int DEFAULT NULL,
  `A 2nd Block 6` int DEFAULT NULL,
  `R 2nd Block 5` int DEFAULT NULL,
  `R 2nd Block 6` int DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=343 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `practice`
--

DROP TABLE IF EXISTS `practice`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `practice` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `OriginalID` int DEFAULT NULL,
  `Surgery` text,
  `SurgeryInUse` bit(1) DEFAULT NULL,
  `GP1` varchar(100) DEFAULT NULL,
  `GP1Email` varchar(100) DEFAULT NULL,
  `Address1` varchar(500) DEFAULT NULL,
  `Address2` varchar(500) DEFAULT NULL,
  `Town` varchar(100) DEFAULT NULL,
  `Postcode` varchar(100) DEFAULT NULL,
  `Telephone` varchar(100) DEFAULT NULL,
  `Fax` varchar(100) DEFAULT NULL,
  `PracticeManager` varchar(100) DEFAULT NULL,
  `PMEmail` varchar(100) DEFAULT NULL,
  `GP2` varchar(100) DEFAULT NULL,
  `GP2Email` varchar(100) DEFAULT NULL,
  `Website` varchar(100) DEFAULT NULL,
  `GP3` varchar(100) DEFAULT NULL,
  `GP3Email` varchar(100) DEFAULT NULL,
  `GP4` varchar(100) DEFAULT NULL,
  `GP4Email` varchar(100) DEFAULT NULL,
  `AdditionalEmails` varchar(500) DEFAULT NULL,
  `SupplierNumber` int DEFAULT NULL,
  `ContactSurgery` bit(1) DEFAULT NULL,
  `Notes` text,
  `AttachmentsAllocated` text,
  `ContractReceived` int DEFAULT NULL,
  `UCCTNotes` text,
  `QualityVisitDateR1` datetime DEFAULT NULL,
  `QualityVisitNotes` text,
  `Active` int DEFAULT NULL,
  `Disabled` int DEFAULT NULL,
  `Queried` int DEFAULT NULL,
  `ListSize` varchar(200) DEFAULT NULL,
  `NewPractice` bit(1) DEFAULT NULL,
  `AcademicYear` varchar(100) DEFAULT NULL,
  `QualityVisitDate` datetime DEFAULT NULL,
  `OKtoProceed` int DEFAULT NULL,
  `DataReviewDate` datetime DEFAULT NULL,
  `TutorTrainingGPName` text,
  `TutorTrainingDate` datetime DEFAULT NULL,
  `DateCreated` datetime DEFAULT NULL,
  `DateUpdated` datetime DEFAULT NULL,
  `UpdatedBy` int DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=344 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `practiceexternal`
--

DROP TABLE IF EXISTS `practiceexternal`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `practiceexternal` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `PrimaryId` int DEFAULT NULL,
  `OriginalID` int DEFAULT NULL,
  `Surgery` text,
  `SurgeryInUse` bit(1) DEFAULT NULL,
  `GP1` varchar(100) DEFAULT NULL,
  `GP1Email` varchar(100) DEFAULT NULL,
  `Address1` varchar(500) DEFAULT NULL,
  `Address2` varchar(500) DEFAULT NULL,
  `Town` varchar(100) DEFAULT NULL,
  `Postcode` varchar(100) DEFAULT NULL,
  `Telephone` varchar(100) DEFAULT NULL,
  `Fax` varchar(100) DEFAULT NULL,
  `PracticeManager` varchar(100) DEFAULT NULL,
  `PMEmail` varchar(100) DEFAULT NULL,
  `GP2` varchar(100) DEFAULT NULL,
  `GP2Email` varchar(100) DEFAULT NULL,
  `Website` varchar(100) DEFAULT NULL,
  `GP3` varchar(100) DEFAULT NULL,
  `GP3Email` varchar(100) DEFAULT NULL,
  `GP4` varchar(100) DEFAULT NULL,
  `GP4Email` varchar(100) DEFAULT NULL,
  `AdditionalEmails` varchar(500) DEFAULT NULL,
  `SupplierNumber` int DEFAULT NULL,
  `ContactSurgery` bit(1) DEFAULT NULL,
  `Notes` text,
  `AttachmentsAllocated` text,
  `ContractReceived` int DEFAULT NULL,
  `UCCTNotes` text,
  `QualityVisitDateR1` datetime DEFAULT NULL,
  `QualityVisitNotes` text,
  `Active` int DEFAULT NULL,
  `Disabled` int DEFAULT NULL,
  `Queried` int DEFAULT NULL,
  `ListSize` int DEFAULT NULL,
  `NewPractice` bit(1) DEFAULT NULL,
  `AcademicYear` varchar(100) DEFAULT NULL,
  `QualityVisitDate` datetime DEFAULT NULL,
  `OKtoProceed` int DEFAULT NULL,
  `DataReviewDate` datetime DEFAULT NULL,
  `TutorTrainingGPName` text,
  `TutorTrainingDate` datetime DEFAULT NULL,
  `DateCreated` datetime DEFAULT NULL,
  `DateUpdated` datetime DEFAULT NULL,
  `UpdatedBy` int DEFAULT NULL,
  `DateRequested` datetime DEFAULT NULL,
  `RequestedBy` int DEFAULT NULL,
  `DateApproved` datetime DEFAULT NULL,
  `ApprovedBy` int DEFAULT NULL,
  `ChangesApproved` bit(1) DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `qualityvisit`
--

DROP TABLE IF EXISTS `qualityvisit`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `qualityvisit` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `PracticeId` int DEFAULT NULL,
  `VisitTypeId` int DEFAULT NULL,
  `DateOfVisit` datetime DEFAULT NULL,
  `VisitNumber` int DEFAULT NULL,
  `DateCreated` datetime DEFAULT NULL,
  `DateUpdated` datetime DEFAULT NULL,
  `UpdatedBy` int DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `signupdates`
--

DROP TABLE IF EXISTS `signupdates`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `signupdates` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Year2Wk1From` datetime DEFAULT NULL,
  `Year2Wk1To` datetime DEFAULT NULL,
  `Year2Wk2From` datetime DEFAULT NULL,
  `Year2Wk2To` datetime DEFAULT NULL,
  `Year2Wk3From` datetime DEFAULT NULL,
  `Year2Wk3To` datetime DEFAULT NULL,
  `Year2Wk4From` datetime DEFAULT NULL,
  `Year2Wk4To` datetime DEFAULT NULL,
  `Year2Wk5From` datetime DEFAULT NULL,
  `Year2Wk5To` datetime DEFAULT NULL,
  `Year2Wk6From` datetime DEFAULT NULL,
  `Year2Wk6To` datetime DEFAULT NULL,
  `Year3B1From` datetime DEFAULT NULL,
  `Year3B1To` datetime DEFAULT NULL,
  `Year3B2From` datetime DEFAULT NULL,
  `Year3B2To` datetime DEFAULT NULL,
  `Year3B3From` datetime DEFAULT NULL,
  `Year3B3To` datetime DEFAULT NULL,
  `Year3B4From` datetime DEFAULT NULL,
  `Year3B4To` datetime DEFAULT NULL,
  `Year3B5From` datetime DEFAULT NULL,
  `Year3B5To` datetime DEFAULT NULL,
  `Year3B6From` datetime DEFAULT NULL,
  `Year3B6To` datetime DEFAULT NULL,
  `Year3B7From` datetime DEFAULT NULL,
  `Year3B7To` datetime DEFAULT NULL,
  `Year4B1From` datetime DEFAULT NULL,
  `Year4B1To` datetime DEFAULT NULL,
  `Year4B2From` datetime DEFAULT NULL,
  `Year4B2To` datetime DEFAULT NULL,
  `Year4B3From` datetime DEFAULT NULL,
  `Year4B3To` datetime DEFAULT NULL,
  `Year4B4From` datetime DEFAULT NULL,
  `Year4B4To` datetime DEFAULT NULL,
  `Year4B5From` datetime DEFAULT NULL,
  `Year4B5To` datetime DEFAULT NULL,
  `Year4B6From` datetime DEFAULT NULL,
  `Year4B6To` datetime DEFAULT NULL,
  `Year4B7From` datetime DEFAULT NULL,
  `Year4B7To` datetime DEFAULT NULL,
  `Year4B8From` datetime DEFAULT NULL,
  `Year4B8To` datetime DEFAULT NULL,
  `Year5B1From` datetime DEFAULT NULL,
  `Year5B1To` datetime DEFAULT NULL,
  `Year5B2From` datetime DEFAULT NULL,
  `Year5B2To` datetime DEFAULT NULL,
  `Year5B3From` datetime DEFAULT NULL,
  `Year5B3To` datetime DEFAULT NULL,
  `Year5B4From` datetime DEFAULT NULL,
  `Year5B4To` datetime DEFAULT NULL,
  `Year5B5From` datetime DEFAULT NULL,
  `Year5B5To` datetime DEFAULT NULL,
  `Year5B6From` datetime DEFAULT NULL,
  `Year5B6To` datetime DEFAULT NULL,
  `AcademicYear` varchar(45) DEFAULT NULL,
  `DateUpdated` datetime DEFAULT NULL,
  `UpdatedBy` int DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `signupsendlog`
--

DROP TABLE IF EXISTS `signupsendlog`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `signupsendlog` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `SendCode` varchar(45) DEFAULT NULL,
  `AcademicYear` varchar(45) DEFAULT NULL,
  `UserId` int DEFAULT NULL,
  `PracticeId` int DEFAULT NULL,
  `Guid` varchar(150) DEFAULT NULL,
  `NoChangesClicked` bit(1) DEFAULT NULL,
  `DetailsUpdated` bit(1) DEFAULT NULL,
  `DateSent` datetime DEFAULT NULL,
  `DateActionTaken` datetime DEFAULT NULL,
  `SentBy` int DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Table structure for table `user`
--

DROP TABLE IF EXISTS `user`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!50503 SET character_set_client = utf8mb4 */;
CREATE TABLE `user` (
  `Id` int NOT NULL AUTO_INCREMENT,
  `Firstname` varchar(100) DEFAULT NULL,
  `Surname` varchar(100) DEFAULT NULL,
  `Email` varchar(100) DEFAULT NULL,
  `Username` varchar(100) DEFAULT NULL,
  `Pwd` varchar(45) DEFAULT NULL,
  `UserType` int DEFAULT NULL,
  `Year2` bit(1) DEFAULT NULL,
  `Year3` bit(1) DEFAULT NULL,
  `Year4` bit(1) DEFAULT NULL,
  `Year5` bit(1) DEFAULT NULL,
  `PracticeId` int DEFAULT NULL,
  `IsActive` bit(1) DEFAULT NULL,
  `DateCreated` datetime DEFAULT NULL,
  `DateUpdated` datetime DEFAULT NULL,
  `UpdatedBy` int DEFAULT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=25 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;
/*!40101 SET character_set_client = @saved_cs_client */;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2023-05-18 11:10:53
