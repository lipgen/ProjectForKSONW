DELIMITER //
CREATE PROCEDURE insert_документип(
	IN _ID_ДокИП VARCHAR(36),
    IN _ДатаСост VARCHAR(10),
    IN _ДатаВклМСП VARCHAR(10))
BEGIN
	INSERT
		INTO документип
        SET ID_ДокИП = _ID_ДокИП,
			ДатаСост = _ДатаСост,
            ДатаВклМСП = _ДатаВклМСП
        ON DUPLICATE KEY UPDATE
			ДатаСост = _ДатаСост,
			ДатаВклМСП = _ДатаВклМСП;
END //
DELIMITER ;
