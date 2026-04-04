CREATE TABLE IF NOT EXISTS `perfis` (
    `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    `nome` VARCHAR(100) NOT NULL,
    `descricao` TEXT NULL,
    `ativo` TINYINT(1) NOT NULL DEFAULT 1,
    `dataCadastro` DATETIME NOT NULL,
    `dataAtualizacao` DATETIME NULL,
    CONSTRAINT `pk_perfis` PRIMARY KEY (`id`),
    CONSTRAINT `uk_perfis_nome` UNIQUE (`nome`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `empresas` (
    `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    `nomeFantasia` VARCHAR(150) NOT NULL,
    `razaoSocial` VARCHAR(200) NOT NULL,
    `cnpj` VARCHAR(14) NOT NULL,
    `codigoInterno` VARCHAR(50) NULL,
    `email` VARCHAR(150) NOT NULL,
    `telefone` VARCHAR(20) NULL,
    `ativo` TINYINT(1) NOT NULL DEFAULT 1,
    `dataCadastro` DATETIME NOT NULL,
    `dataAtualizacao` DATETIME NULL,
    CONSTRAINT `pk_empresas` PRIMARY KEY (`id`),
    CONSTRAINT `uk_empresas_cnpj` UNIQUE (`cnpj`),
    CONSTRAINT `uk_empresas_codigoInterno` UNIQUE (`codigoInterno`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `tiposDocumento` (
    `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    `nome` VARCHAR(100) NOT NULL,
    `descricao` TEXT NULL,
    `ativo` TINYINT(1) NOT NULL DEFAULT 1,
    `dataCadastro` DATETIME NOT NULL,
    CONSTRAINT `pk_tiposDocumento` PRIMARY KEY (`id`),
    CONSTRAINT `uk_tiposDocumento_nome` UNIQUE (`nome`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `usuarios` (
    `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    `perfilId` BIGINT UNSIGNED NOT NULL,
    `nome` VARCHAR(150) NOT NULL,
    `email` VARCHAR(150) NOT NULL,
    `login` VARCHAR(80) NOT NULL,
    `senhaHash` VARCHAR(255) NOT NULL,
    `telefone` VARCHAR(20) NULL,
    `ativo` TINYINT(1) NOT NULL DEFAULT 1,
    `ultimoLogin` DATETIME NULL,
    `dataCadastro` DATETIME NOT NULL,
    `dataAtualizacao` DATETIME NULL,
    CONSTRAINT `pk_usuarios` PRIMARY KEY (`id`),
    CONSTRAINT `uk_usuarios_email` UNIQUE (`email`),
    CONSTRAINT `uk_usuarios_login` UNIQUE (`login`),
    CONSTRAINT `fk_usuarios_perfis` FOREIGN KEY (`perfilId`) REFERENCES `perfis` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `usuarioEmpresa` (
    `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    `usuarioId` BIGINT UNSIGNED NOT NULL,
    `empresaId` BIGINT UNSIGNED NOT NULL,
    `ativo` TINYINT(1) NOT NULL DEFAULT 1,
    `dataCadastro` DATETIME NOT NULL,
    CONSTRAINT `pk_usuarioEmpresa` PRIMARY KEY (`id`),
    CONSTRAINT `uk_usuarioEmpresa_usuario_empresa` UNIQUE (`usuarioId`, `empresaId`),
    CONSTRAINT `fk_usuarioEmpresa_usuarios` FOREIGN KEY (`usuarioId`) REFERENCES `usuarios` (`id`) ON DELETE CASCADE,
    CONSTRAINT `fk_usuarioEmpresa_empresas` FOREIGN KEY (`empresaId`) REFERENCES `empresas` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `recebimentos` (
    `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    `empresaId` BIGINT UNSIGNED NOT NULL,
    `usuarioEnvioId` BIGINT UNSIGNED NOT NULL,
    `codigoRecebimento` VARCHAR(50) NOT NULL,
    `statusRecebimento` VARCHAR(30) NOT NULL,
    `origem` VARCHAR(30) NOT NULL,
    `observacao` TEXT NULL,
    `dataEnvio` DATETIME NOT NULL,
    `dataRecebimento` DATETIME NOT NULL,
    `dataAtualizacao` DATETIME NOT NULL,
    CONSTRAINT `pk_recebimentos` PRIMARY KEY (`id`),
    CONSTRAINT `uk_recebimentos_codigoRecebimento` UNIQUE (`codigoRecebimento`),
    CONSTRAINT `fk_recebimentos_empresas` FOREIGN KEY (`empresaId`) REFERENCES `empresas` (`id`),
    CONSTRAINT `fk_recebimentos_usuarios` FOREIGN KEY (`usuarioEnvioId`) REFERENCES `usuarios` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `arquivos` (
    `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    `recebimentoId` BIGINT UNSIGNED NOT NULL,
    `tipoDocumentoId` BIGINT UNSIGNED NOT NULL,
    `nomeOriginal` VARCHAR(255) NOT NULL,
    `nomeArquivo` VARCHAR(255) NOT NULL,
    `caminhoArquivo` VARCHAR(500) NOT NULL,
    `extensao` VARCHAR(20) NULL,
    `mimeType` VARCHAR(100) NULL,
    `tamanhoBytes` BIGINT NOT NULL,
    `ordemExibicao` INT NOT NULL DEFAULT 0,
    `ativo` TINYINT(1) NOT NULL DEFAULT 1,
    `dataUpload` DATETIME NOT NULL,
    CONSTRAINT `pk_arquivos` PRIMARY KEY (`id`),
    CONSTRAINT `fk_arquivos_recebimentos` FOREIGN KEY (`recebimentoId`) REFERENCES `recebimentos` (`id`) ON DELETE CASCADE,
    CONSTRAINT `fk_arquivos_tiposDocumento` FOREIGN KEY (`tipoDocumentoId`) REFERENCES `tiposDocumento` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `notasFiscais` (
    `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    `recebimentoId` BIGINT UNSIGNED NOT NULL,
    `arquivoId` BIGINT UNSIGNED NULL,
    `numeroNota` VARCHAR(50) NULL,
    `serie` VARCHAR(20) NULL,
    `chaveAcesso` VARCHAR(60) NULL,
    `valorTotal` DECIMAL(15,2) NOT NULL,
    `dataEmissao` DATE NULL,
    `dataEntrada` DATE NULL,
    `cpfCnpjEmitente` VARCHAR(18) NULL,
    `nomeEmitente` VARCHAR(150) NULL,
    `observacao` TEXT NULL,
    `dataCadastro` DATETIME NOT NULL,
    `dataAtualizacao` DATETIME NULL,
    CONSTRAINT `pk_notasFiscais` PRIMARY KEY (`id`),
    CONSTRAINT `uk_notasFiscais_chaveAcesso` UNIQUE (`chaveAcesso`),
    CONSTRAINT `fk_notasFiscais_recebimentos` FOREIGN KEY (`recebimentoId`) REFERENCES `recebimentos` (`id`) ON DELETE CASCADE,
    CONSTRAINT `fk_notasFiscais_arquivos` FOREIGN KEY (`arquivoId`) REFERENCES `arquivos` (`id`) ON DELETE SET NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `boletos` (
    `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    `recebimentoId` BIGINT UNSIGNED NOT NULL,
    `arquivoId` BIGINT UNSIGNED NULL,
    `codigoBarras` VARCHAR(60) NULL,
    `linhaDigitavel` VARCHAR(80) NULL,
    `valorBoleto` DECIMAL(15,2) NOT NULL,
    `dataVencimento` DATE NULL,
    `dataEmissao` DATE NULL,
    `beneficiario` VARCHAR(150) NULL,
    `documentoBeneficiario` VARCHAR(18) NULL,
    `observacao` TEXT NULL,
    `dataCadastro` DATETIME NOT NULL,
    `dataAtualizacao` DATETIME NULL,
    CONSTRAINT `pk_boletos` PRIMARY KEY (`id`),
    CONSTRAINT `fk_boletos_recebimentos` FOREIGN KEY (`recebimentoId`) REFERENCES `recebimentos` (`id`) ON DELETE CASCADE,
    CONSTRAINT `fk_boletos_arquivos` FOREIGN KEY (`arquivoId`) REFERENCES `arquivos` (`id`) ON DELETE SET NULL,
    INDEX `ix_boletos_linhaDigitavel` (`linhaDigitavel`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `notaFiscalBoleto` (
    `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    `notaFiscalId` BIGINT UNSIGNED NOT NULL,
    `boletoId` BIGINT UNSIGNED NOT NULL,
    `dataCadastro` DATETIME NOT NULL,
    CONSTRAINT `pk_notaFiscalBoleto` PRIMARY KEY (`id`),
    CONSTRAINT `uk_notaFiscalBoleto_nota_boleto` UNIQUE (`notaFiscalId`, `boletoId`),
    CONSTRAINT `fk_notaFiscalBoleto_notasFiscais` FOREIGN KEY (`notaFiscalId`) REFERENCES `notasFiscais` (`id`) ON DELETE CASCADE,
    CONSTRAINT `fk_notaFiscalBoleto_boletos` FOREIGN KEY (`boletoId`) REFERENCES `boletos` (`id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `recebimentoConferencias` (
    `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    `recebimentoId` BIGINT UNSIGNED NOT NULL,
    `usuarioConferenciaId` BIGINT UNSIGNED NOT NULL,
    `statusConferencia` VARCHAR(30) NOT NULL,
    `notaEncontrada` TINYINT(1) NOT NULL DEFAULT 0,
    `boletoEncontrado` TINYINT(1) NOT NULL DEFAULT 0,
    `valorConfere` TINYINT(1) NOT NULL DEFAULT 0,
    `dataVencimentoConfere` TINYINT(1) NOT NULL DEFAULT 0,
    `documentoConfere` TINYINT(1) NOT NULL DEFAULT 0,
    `observacao` TEXT NULL,
    `dataConferencia` DATETIME NOT NULL,
    `dataCadastro` DATETIME NOT NULL,
    CONSTRAINT `pk_recebimentoConferencias` PRIMARY KEY (`id`),
    CONSTRAINT `fk_recebimentoConferencias_recebimentos` FOREIGN KEY (`recebimentoId`) REFERENCES `recebimentos` (`id`) ON DELETE CASCADE,
    CONSTRAINT `fk_recebimentoConferencias_usuarios` FOREIGN KEY (`usuarioConferenciaId`) REFERENCES `usuarios` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `recebimentoDivergencias` (
    `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    `recebimentoId` BIGINT UNSIGNED NOT NULL,
    `usuarioId` BIGINT UNSIGNED NOT NULL,
    `tipoDivergencia` VARCHAR(50) NOT NULL,
    `descricao` TEXT NOT NULL,
    `resolvida` TINYINT(1) NOT NULL DEFAULT 0,
    `dataResolucao` DATETIME NULL,
    `usuarioResolucaoId` BIGINT UNSIGNED NULL,
    `observacaoResolucao` TEXT NULL,
    `dataCadastro` DATETIME NOT NULL,
    `dataAtualizacao` DATETIME NULL,
    CONSTRAINT `pk_recebimentoDivergencias` PRIMARY KEY (`id`),
    CONSTRAINT `fk_recebimentoDivergencias_recebimentos` FOREIGN KEY (`recebimentoId`) REFERENCES `recebimentos` (`id`) ON DELETE CASCADE,
    CONSTRAINT `fk_recebimentoDivergencias_usuario` FOREIGN KEY (`usuarioId`) REFERENCES `usuarios` (`id`),
    CONSTRAINT `fk_recebimentoDivergencias_usuarioResolucao` FOREIGN KEY (`usuarioResolucaoId`) REFERENCES `usuarios` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `recebimentoHistoricos` (
    `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    `recebimentoId` BIGINT UNSIGNED NOT NULL,
    `usuarioId` BIGINT UNSIGNED NULL,
    `acao` VARCHAR(50) NOT NULL,
    `descricao` TEXT NOT NULL,
    `dataCadastro` DATETIME NOT NULL,
    CONSTRAINT `pk_recebimentoHistoricos` PRIMARY KEY (`id`),
    CONSTRAINT `fk_recebimentoHistoricos_recebimentos` FOREIGN KEY (`recebimentoId`) REFERENCES `recebimentos` (`id`) ON DELETE CASCADE,
    CONSTRAINT `fk_recebimentoHistoricos_usuarios` FOREIGN KEY (`usuarioId`) REFERENCES `usuarios` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

CREATE TABLE IF NOT EXISTS `recebimentoComentarios` (
    `id` BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    `recebimentoId` BIGINT UNSIGNED NOT NULL,
    `usuarioId` BIGINT UNSIGNED NOT NULL,
    `comentario` TEXT NOT NULL,
    `visivelParaEmpresa` TINYINT(1) NOT NULL DEFAULT 0,
    `dataCadastro` DATETIME NOT NULL,
    `dataAtualizacao` DATETIME NULL,
    CONSTRAINT `pk_recebimentoComentarios` PRIMARY KEY (`id`),
    CONSTRAINT `fk_recebimentoComentarios_recebimentos` FOREIGN KEY (`recebimentoId`) REFERENCES `recebimentos` (`id`) ON DELETE CASCADE,
    CONSTRAINT `fk_recebimentoComentarios_usuarios` FOREIGN KEY (`usuarioId`) REFERENCES `usuarios` (`id`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;
