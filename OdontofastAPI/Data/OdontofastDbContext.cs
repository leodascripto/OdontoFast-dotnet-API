using Microsoft.EntityFrameworkCore;
using OdontofastAPI.Model;

namespace OdontofastAPI.Data
{
    // Classe que representa o contexto do banco de dados
    public class OdontofastDbContext : DbContext
    {
        // Construtor da classe, recebe as opções de configuração do DbContext
        public OdontofastDbContext(DbContextOptions<OdontofastDbContext> options)
            : base(options)
        {
        }

        // Representação da tabela "Usuarios" no banco de dados
        public DbSet<Usuario> Usuarios { get; set; }

        // Representação da tabela "C_OP_IMG_USUARIO" no banco de dados
        public DbSet<ImagemUsuario> ImagensUsuario { get; set; }

        // Método para configurar as entidades e suas propriedades no banco de dados
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Chama a implementação base do método OnModelCreating
            base.OnModelCreating(modelBuilder);

            // Configuração da entidade Usuario no banco de dados
            modelBuilder.Entity<Usuario>(entity =>
            {
                // Define o nome da tabela no banco de dados
                entity.ToTable("C_OP_USUARIO");

                // Define a chave primária da tabela
                entity.HasKey(u => u.IdUsuario);

                // Configuração da coluna ID_USUARIO
                entity.Property(u => u.IdUsuario)
                    .HasColumnName("ID_USUARIO") // Nome da coluna no banco
                    .HasColumnType("NUMBER(30)") // Tipo de dado no banco
                    .IsRequired(); // Campo obrigatório

                // Configuração da coluna NOME_USUARIO
                entity.Property(u => u.NomeUsuario)
                    .HasColumnName("NOME_USUARIO")
                    .HasColumnType("VARCHAR2(255 CHAR)")
                    .IsRequired();

                // Configuração da coluna SENHA_USUARIO
                entity.Property(u => u.SenhaUsuario)
                    .HasColumnName("SENHA_USUARIO")
                    .HasColumnType("VARCHAR2(255 CHAR)")
                    .IsRequired();

                // Configuração da coluna EMAIL_USUARIO
                entity.Property(u => u.EmailUsuario)
                    .HasColumnName("EMAIL_USUARIO")
                    .HasColumnType("VARCHAR2(255 CHAR)")
                    .IsRequired();

                // Configuração da coluna NR_CARTEIRA
                entity.Property(u => u.NrCarteira)
                    .HasColumnName("NR_CARTEIRA")
                    .HasColumnType("VARCHAR2(255 CHAR)")
                    .IsRequired();

                // Configuração da coluna TELEFONE_USUARIO
                entity.Property(u => u.TelefoneUsuario)
                    .HasColumnName("TELEFONE_USUARIO")
                    .HasColumnType("NUMBER(15)")
                    .IsRequired();
            });

            // No método OnModelCreating, adicione a configuração da entidade:
            modelBuilder.Entity<ImagemUsuario>(entity =>
            {
                // Define o nome da tabela no banco de dados
                entity.ToTable("C_OP_IMG_USUARIO");

                // Define a chave primária da tabela
                entity.HasKey(img => img.IdUsuario);

                // Configuração da coluna ID_USUARIO
                entity.Property(img => img.IdUsuario)
                    .HasColumnName("ID_USUARIO")
                    .HasColumnType("NUMBER(30)")
                    .IsRequired();

                // Configuração da coluna CAMINHO_IMG
                entity.Property(img => img.CaminhoImg)
                    .HasColumnName("CAMINHO_IMG")
                    .HasColumnType("VARCHAR2(500 CHAR)")
                    .IsRequired();

                // Configuração do relacionamento com Usuario
                entity.HasOne(img => img.Usuario)
                    .WithOne()
                    .HasForeignKey<ImagemUsuario>(img => img.IdUsuario)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
