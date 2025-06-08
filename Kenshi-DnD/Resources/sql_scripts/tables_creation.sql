drop database if exists Kenshi_DnD_DB;
create database Kenshi_DnD_DB;
Use Kenshi_DnD_DB;

drop table if exists factions;

create table factions(
	id Int auto_increment primary key,
    name varchar(50) not null,
    description varchar(500),
    baseRelations int not null,
    color int not null,
    respectByFighting bool not null
);
drop table if exists hostilities;
create table hostilities(
	id int primary key auto_increment,
    hostility enum('OkranReligion','CorruptOligarchy','StrengthTest','HiveCastOuts','Survival') not null
);
drop table if exists faction_hostility;
create table faction_hostility(
	idFaction int not null,
	idHostility int not null,
    
    primary key(idFaction,idHostility),
    foreign key(idFaction) references factions(id),
    foreign key(idHostility) references hostilities(id)
);
drop table if exists regions;
create table regions(
	id int auto_increment primary key ,
    name varchar(50) not null,
    description varchar(500) not null,
    hasBar bool default false not null,
    hasShop bool default false not null,
    hasLimbHospital bool default false not null,
    hasContrabandMarket bool default false not null,
    hasRangedShop bool default false not null
);

drop table if exists region_faction;
create table region_faction(
	regionId int not null,
	factionId int not null,
    
    primary key(regionId,factionId),
    foreign key(regionId) references regions(id),
    foreign key(factionId) references factions(id)
);


drop table if exists stats;
create table stats(
	id int auto_increment primary key,
    bruteForce int default 0 not null,
    dexterity int default 0 not null,
    hp int default 0 not null,
    resistance int default 0 not null,
    agility int default 0 not null

);
drop table if exists races;
CREATE TABLE races (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(50) NOT NULL,
    stats_id int UNIQUE not null,
    
    foreign key(stats_id) references stats(id)
);
drop table if exists items;
CREATE TABLE items (
    id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(100) NOT NULL,
    description varchar(200) default '',
    value int not null,
    resellValue int not null,
    type enum('melee','ranged') not null ,
    weight int not null,
    stats_id int Unique not null,
    
    foreign key(stats_id) references stats(id)
    
);
drop table if exists rangedItems;
create table rangedItems(
	item_id int primary key,
    difficulty int not null,
    maxAmmo int not null,
    
    foreign key (item_id) references items(id)
);
drop table if exists meleeItems;
create table meleeItems(
	item_id int primary key,
    breaksOnUse bool default false not null,
    canRevive bool default false not null,
        
    foreign key (item_id) references items(id)
);
drop table if exists limbs;
create table limbs(
	id INT AUTO_INCREMENT PRIMARY KEY,
    name VARCHAR(50) NOT NULL,
    value int not null,
    stats_id int UNIQUE not null,
    
    foreign key(stats_id) references stats(id)
);
drop table if exists enemies;
CREATE TABLE enemies (
    id INT AUTO_INCREMENT PRIMARY KEY,
    factionId int not null,
    name VARCHAR(100) NOT NULL,
    health INT not null,
    strength INT not null,
    resistance INT not null,
    agility int not null,
    immunity enum('None', 'ResistantToRanged','ImmuneToRanged','ImmuneToRangedAndResistantToMelee',
    'ResistantToMelee','ImmuneToMelee','ImmuneToMeleeAndResistantToRanged','ResistantToBoth') not null,
	xp int not null,
    maxCatDrop int not null,
    canDropItem bool default false not null,
    
    FOREIGN KEY (factionId) REFERENCES factions(id)
);
drop table if exists names;
create table names(
	id int auto_increment primary key,
    name varchar(50) not null
);
drop table if exists titles;
CREATE TABLE titles (
    id INT AUTO_INCREMENT PRIMARY KEY,
    title VARCHAR(100) NOT NULL
);
drop table if exists backgrounds;
CREATE TABLE backgrounds (
    id INT AUTO_INCREMENT PRIMARY KEY,
    background VARCHAR(200) NOT NULL
);


