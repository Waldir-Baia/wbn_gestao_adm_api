ALTER TABLE `empresas`
    ADD COLUMN `certificadoDigitalA1`       LONGBLOB         NULL,
    ADD COLUMN `certificadoDigitalSenha`    VARCHAR(500)     NULL,
    ADD COLUMN `certificadoDigitalValidade` DATETIME         NULL,
    ADD COLUMN `certificadoDigitalAtivo`    TINYINT(1)       NOT NULL DEFAULT 0,
    ADD COLUMN `nfeCodigoUf`               SMALLINT UNSIGNED NOT NULL DEFAULT 0,
    ADD COLUMN `nfeUltimoNsu`              BIGINT            NOT NULL DEFAULT 0,
    ADD COLUMN `nfeMaxNsu`                 BIGINT            NOT NULL DEFAULT 0,
    ADD COLUMN `nfeDataUltimaConsulta`     DATETIME         NULL;
