CREATE TABLE `swmtest`.`users` (
  `id` INT NOT NULL,
  `firstName` VARCHAR(255) NULL,
  `lastName` VARCHAR(255) NULL,
  `age` INT NULL,
  `gender` VARCHAR(12) NULL,
  PRIMARY KEY (`id`));

;ALTER TABLE `swmtest`.`users` 
ADD INDEX `age` (`age` ASC);
;

