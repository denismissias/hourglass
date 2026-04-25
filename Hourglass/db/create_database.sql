SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='ONLY_FULL_GROUP_BY,STRICT_TRANS_TABLES,NO_ZERO_IN_DATE,NO_ZERO_DATE,ERROR_FOR_DIVISION_BY_ZERO,NO_ENGINE_SUBSTITUTION';

-- -----------------------------------------------------
-- Schema hourglass
-- -----------------------------------------------------

-- -----------------------------------------------------
-- Schema hourglass
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `hourglass` DEFAULT CHARACTER SET utf8 ;
USE `hourglass` ;

-- -----------------------------------------------------
-- Table `hourglass`.`Users`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `hourglass`.`Users` (
  `user_id` INT NOT NULL AUTO_INCREMENT,
  `login` VARCHAR(32) NOT NULL,
  `password` VARCHAR(128) NOT NULL,
  `name` VARCHAR(256) NOT NULL,
  `email` VARCHAR(256) NOT NULL,
  PRIMARY KEY (`user_id`),
  UNIQUE INDEX `login_UNIQUE` (`login` ASC) VISIBLE)
ENGINE = InnoDB;

INSERT IGNORE INTO `hourglass`.`Users`
SET `login` = 'admin',
`password` = '$2a$11$Pk2tFKnOJNabLai.hcT8A.UHIFXrzpOXWGqFuJxVTlDG3pR2ECSuu',
`name` = 'Admin',
`email` = 'admin@admin.com';

-- -----------------------------------------------------
-- Table `hourglass`.`Projects`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `hourglass`.`Projects` (
  `project_id` INT NOT NULL AUTO_INCREMENT,
  `title` VARCHAR(64) NOT NULL,
  `description` VARCHAR(512) NULL,
  PRIMARY KEY (`project_id`))
ENGINE = InnoDB;

INSERT IGNORE INTO `hourglass`.`Projects` SET `project_id` = 1, `title` = 'Initial Project 1', `description` = 'Initial Project to test 1';
INSERT IGNORE INTO `hourglass`.`Projects` SET `project_id` = 2, `title` = 'Initial Project 2', `description` = 'Initial Project to test 2';
INSERT IGNORE INTO `hourglass`.`Projects` SET `project_id` = 3, `title` = 'Initial Project 3', `description` = 'Initial Project to test 3';
INSERT IGNORE INTO `hourglass`.`Projects` SET `project_id` = 4, `title` = 'Initial Project 4', `description` = 'Initial Project to test 4';
INSERT IGNORE INTO `hourglass`.`Projects` SET `project_id` = 5, `title` = 'Initial Project 5', `description` = 'Initial Project to test 5';
INSERT IGNORE INTO `hourglass`.`Projects` SET `project_id` = 6, `title` = 'Initial Project 6', `description` = 'Initial Project to test 6';
INSERT IGNORE INTO `hourglass`.`Projects` SET `project_id` = 7, `title` = 'Initial Project 7', `description` = 'Initial Project to test 7';
INSERT IGNORE INTO `hourglass`.`Projects` SET `project_id` = 8, `title` = 'Initial Project 8', `description` = 'Initial Project to test 8';
INSERT IGNORE INTO `hourglass`.`Projects` SET `project_id` = 9, `title` = 'Initial Project 9', `description` = 'Initial Project to test 9';
INSERT IGNORE INTO `hourglass`.`Projects` SET `project_id` = 10, `title` = 'Initial Project 10', `description` = 'Initial Project to test 10';

-- -----------------------------------------------------
-- Table `hourglass`.`Times`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `hourglass`.`Times` (
  `time_id` INT NOT NULL AUTO_INCREMENT,
  `project_id` INT NOT NULL,
  `user_id` INT NOT NULL,
  `started_at` DATETIME NOT NULL,
  `ended_at` DATETIME NOT NULL,
  PRIMARY KEY (`time_id`),
  INDEX `fk_Time_Project1_idx` (`project_id` ASC) VISIBLE,
  INDEX `fk_Time_User1_idx` (`user_id` ASC) VISIBLE,
  CONSTRAINT `fk_Time_Project1`
    FOREIGN KEY (`project_id`)
    REFERENCES `hourglass`.`Projects` (`project_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_Time_User1`
    FOREIGN KEY (`user_id`)
    REFERENCES `hourglass`.`Users` (`user_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


-- -----------------------------------------------------
-- Table `hourglass`.`Users_By_Projects`
-- -----------------------------------------------------
CREATE TABLE IF NOT EXISTS `hourglass`.`Users_By_Projects` (
  `user_id` INT NOT NULL,
  `project_id` INT NOT NULL,
  PRIMARY KEY (`user_id`, `project_id`),
  INDEX `fk_User_has_Project_Project1_idx` (`project_id` ASC) VISIBLE,
  INDEX `fk_User_has_Project_User_idx` (`user_id` ASC) VISIBLE,
  CONSTRAINT `fk_User_has_Project_User`
    FOREIGN KEY (`user_id`)
    REFERENCES `hourglass`.`Users` (`user_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_User_has_Project_Project1`
    FOREIGN KEY (`project_id`)
    REFERENCES `hourglass`.`Projects` (`project_id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;