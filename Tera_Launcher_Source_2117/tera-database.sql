/*
Source Server         : localhost
Source Server Version : 50529
Source Host           : localhost:3306
Source Database       : tera
Target Server Type    : MYSQL
Target Server Version : 50529
File Encoding         : 65001
Date: 2013-05-07 17:34:18
*/
SET FOREIGN_KEY_CHECKS=0;

-- ----------------------------
-- Table structure for `Accounts`
-- ----------------------------
DROP TABLE IF EXISTS `Accounts`;
CREATE TABLE `Accounts` (
  `AccountId` int(11) NOT NULL AUTO_INCREMENT,
  `AccountName` varchar(50) NOT NULL,
  `AccountPassword` varchar(512) NOT NULL,
  `AccountEmail` varchar(50) NOT NULL,
  `AccessLevel` int(1) NOT NULL DEFAULT '0',
  `Membership` int(1) NOT NULL DEFAULT '0',
  `isGM` int(1) NOT NULL DEFAULT '0',
  `LastOnlineUtc` bigint(100) NOT NULL DEFAULT '0',
  `Coins` int(11) NOT NULL DEFAULT '0',
  `UISettings` longtext NOT NULL,
  PRIMARY KEY (`AccountId`)
) ENGINE=InnoDB AUTO_INCREMENT=2 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for `character`
-- ----------------------------
DROP TABLE IF EXISTS `Player`;
CREATE TABLE `Player` (
  `PlayerId` int(11) NOT NULL AUTO_INCREMENT,
  `AccountName` varchar(50) NOT NULL,
  `PlayerLevel` int(11) NOT NULL,
  `PlayerExp` bigint(20) NOT NULL,
  `PlayerExpRecoverable` bigint(20) NOT NULL,
  `PlayerMount` int(11) NOT NULL,
  `PlayerGuildAccepted` bit(1) NOT NULL,
  `PlayerPraiseGiven` bit(1) NOT NULL,
  `PlayerLastPraise` int(11) NOT NULL DEFAULT '-1',
  `PlayerCurrentBankSection` int(11) NOT NULL,
  `PlayerCreationDate` int(11) NOT NULL,
  `PlayerLastOnlineUtc` int(11) NOT NULL DEFAULT '0',
  `PlayerDeleted` smallint(6) DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for `character_data`
-- ----------------------------
DROP TABLE IF EXISTS `PlayerData`;
CREATE TABLE `PlayerData` (
  `PlayerId` int(11) NOT NULL AUTO_INCREMENT,
  `PlayerName` varchar(50) NOT NULL,
  `PlayerGender` enum('Male','Female','','') NOT NULL,
  `PlayerRace` enum('Human','HighElf','Aman','Castanic','Popori','Elin','Baraka') NOT NULL,
  `PlayerClass` enum('Warrior','Lancer','Slayer','Berserker','Sorcerer','Archer','Priest','Mystic','Elementalist') NOT NULL,
  `PlayerData` longtext NOT NULL,
  `PlayerDetails1` longtext NOT NULL,
  `PlayerDetails2` longtext NOT NULL,
  `PlayerMapId` int(11) NOT NULL,
  `PlayerX` float NOT NULL,
  `PlayerY` float NOT NULL,
  `PlayerZ` float NOT NULL,
  `PlayerH` int(11) NOT NULL,
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Table structure for `inventory`
-- ----------------------------
DROP TABLE IF EXISTS `inventory`;
CREATE TABLE `inventory` (
  `Id` bigint(20) NOT NULL AUTO_INCREMENT,
  `AccountName` varchar(50) NOT NULL DEFAULT '',
  `PlayerId` int(11) NOT NULL DEFAULT '0',
  `ItemId` int(11) NOT NULL DEFAULT '0',
  `Amount` int(11) NOT NULL DEFAULT '0',
  `Color` int(11) NOT NULL DEFAULT '0',
  `Slot` int(11) NOT NULL DEFAULT '0',
  `StorageType` enum('Inventory','CharacterWarehouse','AccountWarehouse','GuildWarehouse','Trade') NOT NULL DEFAULT 'Inventory',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8;

-- ----------------------------
-- Table structure for `guilds`
-- ----------------------------
DROP TABLE IF EXISTS `guilds`;
CREATE TABLE `guilds` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `guildname` varchar(50) NOT NULL,
  `guildranks` longtext NOT NULL,
  `guildlogo` longtext CHARACTER SET latin1 COLLATE latin1_bin,
  `guildhistory` longtext,
  `level` int(11) NOT NULL,
  `guildadd` longtext,
  `guildtitle` varchar(50) DEFAULT NULL,
  `guildmessage` text,
  `praises` int(11) NOT NULL DEFAULT '0',
  `creationdate` int(11) NOT NULL,
  `guildmembers` longtext NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Table structure for `guild_events`
-- ----------------------------
DROP TABLE IF EXISTS `guild_events`;
CREATE TABLE `guild_events` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `gid` int(11) NOT NULL,
  `initiator` varchar(50) NOT NULL,
  `date` int(11) NOT NULL,
  `args` longtext NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Table structure for `guild_ranks`
-- ----------------------------
DROP TABLE IF EXISTS `guild_ranks`;
CREATE TABLE `guild_ranks` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `gid` int(11) NOT NULL,
  `rankprivileges` int(11) NOT NULL,
  `rankname` varchar(50) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Table structure for `inventory`
-- ----------------------------
DROP TABLE IF EXISTS `inventory`;
CREATE TABLE `inventory` (
  `accountname` varchar(50) NOT NULL,
  `playerid` int(11) NOT NULL,
  `itemid` int(11) NOT NULL,
  `amount` int(11) NOT NULL,
  `color` int(11) NOT NULL,
  `slot` int(5) NOT NULL,
  `storagetype` enum('Inventory','CharacterWarehouse','AccountWarehouse','GuildWarehouse','Trade') NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Table structure for `quests`
-- ----------------------------
DROP TABLE IF EXISTS `quests`;
CREATE TABLE `quests` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `characterid` int(11) NOT NULL,
  `questid` int(11) NOT NULL,
  `status` enum('None','Start','Reward','Complete','Locked') NOT NULL,
  `step` int(11) NOT NULL,
  `counters` int(11) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

-- ----------------------------
-- Table structure for `skills`
-- ----------------------------
DROP TABLE IF EXISTS `skills`;
CREATE TABLE `skills` (
  `Id` int(11) NOT NULL AUTO_INCREMENT,
  `PlayerId` int(11) NOT NULL DEFAULT '0',
  `SkillId` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;