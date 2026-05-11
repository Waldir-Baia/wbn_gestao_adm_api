-- ============================================================
-- SEED INICIAL — primeiro acesso ao sistema
-- Senha do usuário admin: Admin@123
-- ============================================================

-- 1. Perfil administrador
INSERT INTO `perfis` (`nome`, `descricao`, `ativo`, `dataCadastro`)
VALUES ('Administrador', 'Perfil com acesso total ao sistema.', 1, NOW());

-- 2. Empresa inicial
INSERT INTO `empresas` (`nomeFantasia`, `razaoSocial`, `cnpj`, `codigoInterno`, `email`, `telefone`, `ativo`, `dataCadastro`)
VALUES ('Empresa Demo', 'Empresa Demo Ltda', '00000000000191', 'DEMO001', 'contato@empresa-demo.com.br', NULL, 1, NOW());

-- 3. Usuário administrador
--    Senha: Admin@123  (hash PBKDF2-SHA256 100.000 iterações)
INSERT INTO `usuarios` (`perfilId`, `nome`, `email`, `login`, `senhaHash`, `telefone`, `ativo`, `statusSenha`, `dataCadastro`)
VALUES (
    (SELECT `id` FROM `perfis` WHERE `nome` = 'Administrador' LIMIT 1),
    'Administrador',
    'admin@empresa-demo.com.br',
    'admin',
    '100000.tzKEvugSPKr0Fgy75/dedQ==.lDLCT0EPG92wSXdRNwU91bcB2DMz3F1jyTaPp3OKOy0=',
    NULL,
    1,
    0,
    NOW()
);

-- 4. Vínculo usuário ↔ empresa
INSERT INTO `usuarioEmpresa` (`usuarioId`, `empresaId`, `ativo`, `dataCadastro`)
VALUES (
    (SELECT `id` FROM `usuarios` WHERE `email` = 'admin@empresa-demo.com.br' LIMIT 1),
    (SELECT `id` FROM `empresas` WHERE `codigoInterno` = 'DEMO001' LIMIT 1),
    1,
    NOW()
);
